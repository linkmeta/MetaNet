using MetaNet.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using MetaNet.Interfaces;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace MetaNet.ViewModels
{
    internal class UdpViewModel : MainWindowBase
    {
        public UdpModel UdpModel { get; set; }

        #region TcpServer
        UdpServerSocket udpServerSocket = null;

        public class UdpClientInfo
        {
            public string RemoteIp { get; set; }
            public string Port { get; set; }
            public int RecvBytes { get; set; }
            public DateTime Time { get; set; }
        }
        public ObservableCollection<UdpClientInfo> UdpClientInfos { get; set; } = new ObservableCollection<UdpClientInfo> { };

        public ICommand StartListenCommand
        {
            get
            {
                return new RelayCommand(param => StartListen(param));
            }
        }
        public void StartListen(object parameter)
        {
            if (UdpModel.ServerListenBtnName == "Start Listen")
            {
                UdpModel.ServerListenBtnName = "Stop Listen";

                udpServerSocket = new UdpServerSocket(IPAddress.Any.ToString(), UdpModel.ListenPort);
                udpServerSocket.recvEvent = new Action<EndPoint,string,int>(Recv);
                udpServerSocket.Start();
                UdpModel.ServerStatus += "Udp Server Started!\n";
            }
            else
            {
                UdpModel.ServerListenBtnName = "Start Listen";
                UdpClientInfos.Clear();
                UdpModel.ServerStatus += "Udp Server Stopped!\n";

            }
        }
        private void Recv(EndPoint point, string message, int len)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UdpModel.ServerRecv += "[" + point.ToString() + "] :";
                UdpModel.ServerRecv += message;
                UdpModel.ServerRecv += "\n";
                DateTime time = DateTime.Now;
                UdpClientInfos.Add(new UdpClientInfo { RemoteIp = point.ToString().Split(':')[0], Port = point.ToString().Split(':')[1], RecvBytes = len, Time = time });
                UdpModel.ServerStatus += "++[" + point.ToString() + "] connected at " + time + "\n";
            }));

        }

        public ICommand ServerAutoSendCommand
        {
            get
            {
                return new RelayCommand(param => ServerAutoSend(param));
            }
        }
        private System.Windows.Threading.DispatcherTimer mServerAutoSendTimer;
        /// <param name="e"></param>
        private void ServerAutoSendTimerFunc(object sender, EventArgs e)
        {
            udpServerSocket.SendMessageToAllClientsAsync(UdpModel.ServerSendStr);
        }

        public void ServerAutoSend(object parameter)
        {
            if (UdpModel.ServerSendBtnName == "Auto Send Start")
            {
                UdpModel.ServerSendBtnName = "Auto Send Stop";
                mServerAutoSendTimer = new System.Windows.Threading.DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 0, UdpModel.ServerSendInterval)
                };

                mServerAutoSendTimer.Tick += ServerAutoSendTimerFunc;
                mServerAutoSendTimer.Start();

            }
            else
            {
                UdpModel.ServerSendBtnName = "Auto Send Start";
                mServerAutoSendTimer.Stop();
            }
        }

        public ICommand ServerRecvClearCommand
        {
            get
            {
                return new RelayCommand(param => ServerRecvClear(param));
            }
        }
        public void ServerRecvClear(object parameter)
        {
            UdpModel.ServerRecv = "";
        }

        public ICommand ServerSendClearCommand
        {
            get
            {
                return new RelayCommand(param => ServerSendClear(param));
            }
        }
        public void ServerSendClear(object parameter)
        {
            UdpModel.ServerSend = "";
        }
        public ICommand ServerSendCommand
        {
            get
            {
                return new RelayCommand(param => ServerSend(param));
            }
        }
        public void ServerSend(object parameter)
        {
            udpServerSocket.SendMessageToAllClientsAsync(UdpModel.ServerSend);
        }
        #endregion

        #region UdpClient
        private UdpClientSocket udpClientSocket = null;
        public ICommand ClientConnectCommand
        {
            get
            {
                return new RelayCommand(param => ClientConnect(param));
            }
        }
        public void ClientConnect(object parameter)
        {
            if (UdpModel.ClientConnectBtnName == "Connect")
            {
                UdpModel.ClientConnectBtnName = "Disconnect";
                udpClientSocket = new UdpClientSocket(UdpModel.ServerIp, UdpModel.ServerPort);
                udpClientSocket.recvEvent = new Action<string>(ClientRecvCB);
                udpClientSocket.Start();
            }
            else
            {
                UdpModel.ClientConnectBtnName = "Connect";
                udpClientSocket.CloseClientSocket();
            }
        }

        private void ClientConnectCB(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime time = DateTime.Now;
                UdpModel.ClientRecv += "++[" + socket.RemoteEndPoint.ToString() + "] connected at " + time + "\n";
                UdpModel.LocalPort = socket.LocalEndPoint.ToString().Split(':')[1];
            }));
        }
        private void ClientDisConnectCB(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UdpModel.ClientRecv += "--[" + socket.RemoteEndPoint.ToString() + "] disconnected at " + DateTime.Now + "\n";
                if (UdpModel.ClientConnectBtnName == "Disconnect")
                {
                    UdpModel.ClientConnectBtnName = "Connect";
                    udpClientSocket.CloseClientSocket();
                }
            }));

        }
        private void ClientRecvCB(string msg)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UdpModel.ClientRecv += msg;
                UdpModel.ClientRecv += "\n";
            }));

        }
        public ICommand ClientSendClearCommand
        {
            get
            {
                return new RelayCommand(param => ClientSendClear(param));
            }
        }
        public void ClientSendClear(object parameter)
        {
            UdpModel.ClientSend = "";
        }
        public ICommand ClientSendCommand
        {
            get
            {
                return new RelayCommand(param => ClientSend(param));
            }
        }
        public void ClientSend(object parameter)
        {
            udpClientSocket.SendAsync(UdpModel.ClientSend);
        }
        public ICommand ClientAutoSendCommand
        {
            get
            {
                return new RelayCommand(param => ClientAutoSend(param));
            }
        }

        private System.Windows.Threading.DispatcherTimer mClientAutoSendTimer;
        private void ClientAutoSendFunc(object sender, EventArgs e)
        {
            udpClientSocket.SendAsync(UdpModel.ClientSendStr);
        }

        public void ClientAutoSend(object parameter)
        {
            if (UdpModel.ClientSendBtnName == "Auto Send Start")
            {
                UdpModel.ClientSendBtnName = "Auto Send Stop";
                mClientAutoSendTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 0, UdpModel.ClientSendInterval)
                };
                mClientAutoSendTimer.Tick += ClientAutoSendFunc;
                mClientAutoSendTimer.Start();
            }
            else
            {
                UdpModel.ClientSendBtnName = "Auto Send Start";
                mClientAutoSendTimer.Stop();
            }
        }
        public ICommand ClientRecvClearCommand
        {
            get
            {
                return new RelayCommand(param => ClientRecvClear(param));
            }
        }
        public void ClientRecvClear(object parameter)
        {
            UdpModel.ClientRecv = "";
        }

        #endregion
        public UdpViewModel()
        {
            UdpModel = new UdpModel();
        }
    }
}
