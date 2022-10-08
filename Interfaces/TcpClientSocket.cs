using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MetaNet.Interfaces
{
    public class TcpClientSocket
    {
        public Action<Socket> connectEvent = null;
        public Action<Socket> disConnectEvent = null;

        public Action<string> recvEvent = null;
        public Action<int> sendResultEvent = null;

        private Socket connectSocket = null;

        private string hostIp = "";

        private int port = 0;
        private int bufferSize = 1024;

        public TcpClientSocket(string ip, int port)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("host ip cannot be null");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException("port is out of range");

            this.hostIp = ip;
            this.port = port;
        }

        public void Start()
        {
            try
            {
                connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connectSocket.Connect(hostIp, port);
                if (connectEvent != null)
                    connectEvent(connectSocket);
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
                try
                {
                    while ((len = connectSocket.Receive(buffer, bufferSize, SocketFlags.None)) > 0)
                    {
                        if (recvEvent != null)
                            recvEvent(Encoding.UTF8.GetString(buffer, 0, len));
                    }
                    if (disConnectEvent != null)
                        disConnectEvent(connectSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }));
        }

        public async void SendAsync(string message)
        {
            await Task.Run(new Action(() =>
            {
                int len = 0;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                try
                {
                    if ((len = connectSocket.Send(buffer, buffer.Length, SocketFlags.None)) > 0)
                    {
                        if (sendResultEvent != null)
                            sendResultEvent(len);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }));
        }

        public void CloseClientSocket()
        {
            try
            {
                connectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                connectSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Restart()
        {
            CloseClientSocket();
            Start();
        }

    }

}
