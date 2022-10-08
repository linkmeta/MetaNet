using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MetaNet.Interfaces
{
    public class UdpClientSocket
    {
        public Action<Socket> connectEvent = null;
        public Action<Socket> disConnectEvent = null;

        public Action<string> recvEvent = null;
        public Action<int> sendResultEvent = null;

        private Socket connectSocket = null;

        private string hostIp = "";

        private int port = 0;
        private int bufferSize = 1024;

        public UdpClientSocket(string ip, int port)
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
                connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //connectSocket.Connect(hostIp, port);
                connectSocket.Bind(new IPEndPoint(IPAddress.Parse(hostIp), 0));
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
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    while ((len = connectSocket.ReceiveFrom(buffer, ref point)) > 0)
                    {
                        if (recvEvent != null)
                            recvEvent(Encoding.UTF8.GetString(buffer, 0, len));
                    }
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
                EndPoint point = new IPEndPoint(IPAddress.Parse(hostIp), port);
                try
                {
                    if ((len = connectSocket.SendTo(buffer, point)) > 0)
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
