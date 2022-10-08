using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetaNet.Interfaces
{
    public class TcpServerSocket
    {
        public Action<Socket> connectEvent = null;
        public Action<Socket> disConnectEvent = null;

        public Action<Socket, string> recvEvent = null;

        public Action<int> sendResultEvent = null;

        private int numConnections = 65536;

        private Socket listenSocket = null;

        private string hostIp = "";

        private int port = 0;

        private Semaphore maxNumberAcceptedClients = null;
        private int bufferSize = 1024;
        private List<Socket> clientSockets = null;
 
        public TcpServerSocket(string ip, int port)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("host cannot be null");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException("port is out of range");
            if (numConnections <= 0 || numConnections > int.MaxValue)
                throw new ArgumentOutOfRangeException("_numConnections is out of range");

            this.hostIp = ip;
            this.port = port;

            clientSockets = new List<Socket>();
            maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        public void Start()
        {
            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(new IPEndPoint(IPAddress.Parse(hostIp), port));
                listenSocket.Listen(numConnections);
                AcceptAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void AcceptAsync()
        {
            await Task.Run(new Action(() =>
            {
                while (true)
                {
                    try
                    {
                        maxNumberAcceptedClients.WaitOne();
                        Socket acceptSocket = listenSocket.Accept();
                        if (acceptSocket == null)
                            continue;

                        clientSockets.Add(acceptSocket);
                        if (connectEvent != null)
                            connectEvent(acceptSocket);
                        RecvAsync(acceptSocket);
                        maxNumberAcceptedClients.Release();
                    }
                    catch (Exception)
                    {
                        //maxNumberAcceptedClients.Release();
                        Thread.Sleep(1000);
                    }
                }
            }));
        }

        private async void RecvAsync(Socket acceptSocket)
        {
            await Task.Run(new Action(() =>
            {
                int len = 0;
                byte[] buffer = new byte[bufferSize];

                try
                {
                    while ((len = acceptSocket.Receive(buffer, bufferSize, SocketFlags.None)) > 0)
                    {
                        if (recvEvent != null)
                            recvEvent(acceptSocket, Encoding.UTF8.GetString(buffer, 0, len));
                    }
                    if (disConnectEvent != null)
                        disConnectEvent(acceptSocket);
                }
                catch (Exception)
                {
                    CloseClientSocket(acceptSocket);
                }
            }));
        }

        public async void SendAsync(Socket acceptSocket, string message)
        {
            await Task.Run(new Action(() =>
            {
                int len = 0;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                try
                {
                    if ((len = acceptSocket.Send(buffer, buffer.Length, SocketFlags.None)) > 0)
                    {
                        if (sendResultEvent != null)
                            sendResultEvent(len);
                    }
                }
                catch (Exception)
                {
                    CloseClientSocket(acceptSocket);
                }
            }));
        }

        public async void SendMessageToAllClientsAsync(string message)
        {
            await Task.Run(new Action(() =>
            {
                foreach (var socket in clientSockets)
                {
                    SendAsync(socket, message);
                }
            }));
        }

        private void CloseClientSocket(Socket acceptSocket)
        {
            try
            {
                acceptSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            try
            {
                acceptSocket.Close();
            }
            catch { }
            try
            {
                maxNumberAcceptedClients.Release();
            }
            catch { 
            }
        }

        public void CloseAllClientSocket()
        {
            try
            {
                foreach (var socket in clientSockets)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }
            try
            {
                foreach (var socket in clientSockets)
                {
                    socket.Close();
                }
            }
            catch { }

            try
            {
                listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch { }
            try
            {
                listenSocket.Close();
            }
            catch { }

            try
            {
                maxNumberAcceptedClients.Release(clientSockets.Count);
                clientSockets.Clear();
            }
            catch { }
        }
    }
}
