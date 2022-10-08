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
    public class UdpServerSocket
    {

        public Action<EndPoint,string,int> recvEvent = null;

        public Action<int> sendResultEvent = null;

        private int numConnections = 65536;

        private Socket listenSocket = null;

        private string hostIp = "";

        private int port = 0;

        private Semaphore maxNumberAcceptedClients = null;
        private int bufferSize = 1024;
        private List<EndPoint> clientSockets = null;
 
        public UdpServerSocket(string ip, int port)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("host cannot be null");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException("port is out of range");
            if (numConnections <= 0 || numConnections > int.MaxValue)
                throw new ArgumentOutOfRangeException("_numConnections is out of range");

            this.hostIp = ip;
            this.port = port;

            clientSockets = new List<EndPoint>();
            maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        public void Start()
        {
            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listenSocket.Bind(new IPEndPoint(IPAddress.Parse(hostIp), port));

                RecvAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void RecvAsync()
        {
            await Task.Run(new Action(() =>
            {
                int len = 0;
                byte[] buffer = new byte[bufferSize];
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    while ((len = listenSocket.ReceiveFrom(buffer, ref point)) > 0)
                    {
                        if (recvEvent != null)
                            recvEvent( point, Encoding.UTF8.GetString(buffer, 0, len), len);
                        clientSockets.Add(point);
                    }

                }
                catch (Exception)
                {
                    //CloseClientSocket(acceptSocket);
                }
            }));
        }

        public async void SendAsync(EndPoint point, string message)
        {
            await Task.Run(new Action(() =>
            {
                int len = 0;
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                try
                {
                    if ((len = listenSocket.SendTo(buffer, point)) > 0)
                    {
                        if (sendResultEvent != null)
                            sendResultEvent(len);
                    }
                }
                catch (Exception)
                {
                    //CloseClientSocket(acceptSocket);
                }
            }));
        }

        public async void SendMessageToAllClientsAsync(string message)
        {
            await Task.Run(new Action(() =>
            {
                foreach (var point in clientSockets)
                {
                    SendAsync(point, message);
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
                    //socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }
            try
            {
                foreach (var socket in clientSockets)
                {
                    //socket.Close();
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
