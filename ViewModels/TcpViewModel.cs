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
    internal class TcpViewModel : MainWindowBase
    {
        public TcpModel TcpModel { get; set; }

        #region TcpServer
        TcpServerSocket tcpServerSocket = null;

        public class TcpServerInfo
        {
            public string RemoteIp { get; set; }
            public string Port { get; set; }
            public DateTime Time { get; set; }
        }
        public ObservableCollection<TcpServerInfo> TcpServerInfos { get; set; } = new ObservableCollection<TcpServerInfo> { };

        public ICommand StartListenCommand
        {
            get
            {
                return new RelayCommand(param => StartListen(param));
            }
        }
        public void StartListen(object parameter)
        {
            if (TcpModel.ServerListenBtnName == "Start Listen")
            {
                TcpModel.ServerListenBtnName = "Stop Listen";

                tcpServerSocket = new TcpServerSocket(IPAddress.Any.ToString(), TcpModel.ListenPort);
                tcpServerSocket.recvEvent = new Action<Socket, string>(Recv);
                tcpServerSocket.connectEvent = new Action<Socket>(ConnectCallback);
                tcpServerSocket.disConnectEvent = new Action<Socket>(DisConnectCallback);
                tcpServerSocket.Start();
                TcpModel.ServerStatus += "Tcp Server Started!\n";
            }
            else
            {
                TcpModel.ServerListenBtnName = "Start Listen";
                tcpServerSocket.CloseAllClientSocket();
                TcpServerInfos.Clear();
                TcpModel.ServerStatus += "Tcp Server Stopped!\n";

            }
        }
        private void Recv(Socket socket, string message)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TcpModel.ServerRecv += "[" + socket.RemoteEndPoint.ToString() + "] :";
                TcpModel.ServerRecv += message;
                TcpModel.ServerRecv += "\n";
            }));

        }
        private void ConnectCallback(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime time = DateTime.Now;
                TcpServerInfos.Add(new TcpServerInfo {RemoteIp = socket.RemoteEndPoint.ToString().Split(':')[0], Port = socket.RemoteEndPoint.ToString().Split(':')[1], Time = time });
                TcpModel.ServerStatus += "++[" + socket.RemoteEndPoint.ToString() + "] connected at " + time + "\n";
            }));

        }
        private void DisConnectCallback(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (TcpServerInfo info in TcpServerInfos)
                {
                    if (info.RemoteIp == socket.RemoteEndPoint.ToString().Split(':')[0] && info.Port == socket.RemoteEndPoint.ToString().Split(':')[1])
                    {
                        TcpServerInfos.Remove(info);
                        TcpModel.ServerStatus += "--[" + socket.RemoteEndPoint.ToString() + "] disconnected at " + info.Time + "\n";
                        break;
                    }
                }
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
            if (tcpServerSocket!=null)
            {
                tcpServerSocket.SendMessageToAllClientsAsync(TcpModel.ServerSendStr);
            }
        }

        public void ServerAutoSend(object parameter)
        {
            if (TcpModel.ServerSendBtnName == "Auto Send Start")
            {
                TcpModel.ServerSendBtnName = "Auto Send Stop";
                mServerAutoSendTimer = new System.Windows.Threading.DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 0, TcpModel.ServerSendInterval)
                };

                mServerAutoSendTimer.Tick += ServerAutoSendTimerFunc;
                mServerAutoSendTimer.Start();

            }
            else
            {

                TcpModel.ServerSendBtnName = "Auto Send Start";
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
            TcpModel.ServerRecv = "";
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
            TcpModel.ServerSend = "";
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
            if(tcpServerSocket != null)
            {
                tcpServerSocket.SendMessageToAllClientsAsync(TcpModel.ServerSend);
            }

        }
        #endregion

        #region TcpClient
        private TcpClientSocket tcpClientSocket = null;
        public ICommand ClientConnectCommand
        {
            get
            {
                return new RelayCommand(param => ClientConnect(param));
            }
        }
        public void ClientConnect(object parameter)
        {
            if (TcpModel.ClientConnectBtnName == "Connect")
            {
                TcpModel.ClientConnectBtnName = "Disconnect";
                tcpClientSocket = new TcpClientSocket(TcpModel.ServerIp, TcpModel.ServerPort);
                tcpClientSocket.recvEvent = new Action<string>(ClientRecvCB);
                tcpClientSocket.connectEvent = new Action<Socket>(ClientConnectCB);
                tcpClientSocket.disConnectEvent = new Action<Socket>(ClientDisConnectCB);
                tcpClientSocket.Start();
            }
            else
            {
                TcpModel.ClientConnectBtnName = "Connect";
                tcpClientSocket.CloseClientSocket();
            }
        }

        private void ClientConnectCB(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime time = DateTime.Now;
                TcpModel.ClientRecv += "++[" + socket.RemoteEndPoint.ToString() + "] connected at " + time + "\n";
                TcpModel.LocalPort = socket.LocalEndPoint.ToString().Split(':')[1];
            }));
        }
        private void ClientDisConnectCB(Socket socket)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TcpModel.ClientRecv += "--[" + socket.RemoteEndPoint.ToString() + "] disconnected at " + DateTime.Now + "\n";
                if (TcpModel.ClientConnectBtnName == "Disconnect")
                {
                    TcpModel.ClientConnectBtnName = "Connect";
                    tcpClientSocket.CloseClientSocket();
                }
            }));

        }
        private void ClientRecvCB(string msg)
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TcpModel.ClientRecv += msg;
                TcpModel.ClientRecv += "\n";
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
            TcpModel.ClientSend = "";
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
            if(tcpClientSocket != null)
            {
                tcpClientSocket.SendAsync(TcpModel.ClientSend);
            }

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
            if(tcpClientSocket != null)
            {
                tcpClientSocket.SendAsync(TcpModel.ClientSendStr);
            }

        }

        public void ClientAutoSend(object parameter)
        {
            if (TcpModel.ClientSendBtnName == "Auto Send Start")
            {
                TcpModel.ClientSendBtnName = "Auto Send Stop";
                mClientAutoSendTimer = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 0, 0, TcpModel.ClientSendInterval)
                };
                mClientAutoSendTimer.Tick += ClientAutoSendFunc;
                mClientAutoSendTimer.Start();
            }
            else
            {
                TcpModel.ClientSendBtnName = "Auto Send Start";
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
            TcpModel.ClientRecv = "";
        }

        #endregion
        public TcpViewModel()
        {
            TcpModel = new TcpModel();
        }
    }
}
