using MetaNet.ViewModels;

namespace MetaNet.Models
{
    public class TcpModel : MainWindowBase
    {
        #region Tcp Server
        private int _ListenPort;
        public int ListenPort
        {
            get { return _ListenPort; }
            set
            {
                if (_ListenPort != value)
                {
                    _ListenPort = value;
                    OnPropertyChanged(nameof(ListenPort));
                }
            }
        }
        private string _ServerListenBtnName;
        public string ServerListenBtnName
        {
            get { return _ServerListenBtnName; }
            set
            {
                if (_ServerListenBtnName != value)
                {
                    _ServerListenBtnName = value;
                    OnPropertyChanged(nameof(ServerListenBtnName));
                }
            }
        }
        private string _ServerSendStr;
        public string ServerSendStr
        {
            get { return _ServerSendStr; }
            set
            {
                if (_ServerSendStr != value)
                {
                    _ServerSendStr = value;
                    OnPropertyChanged(nameof(ServerSendStr));
                }
            }
        }

        private int _ServerSendInterval;
        public int ServerSendInterval
        {
            get { return _ServerSendInterval; }
            set
            {
                if (_ServerSendInterval != value)
                {
                    _ServerSendInterval = value;
                    OnPropertyChanged(nameof(ServerSendInterval));
                }
            }
        }

        private string _ServerSendBtnName;
        public string ServerSendBtnName
        {
            get { return _ServerSendBtnName; }
            set
            {
                if (_ServerSendBtnName != value)
                {
                    _ServerSendBtnName = value;
                    OnPropertyChanged(nameof(ServerSendBtnName));
                }
            }
        }

        private string _ServerStatus;
        public string ServerStatus
        {
            get { return _ServerStatus; }
            set
            {
                if (_ServerStatus != value)
                {
                    _ServerStatus = value;
                    OnPropertyChanged(nameof(ServerStatus));
                }
            }
        }

        private string _ServerRecv;
        public string ServerRecv
        {
            get { return _ServerRecv; }
            set
            {
                if (_ServerRecv != value)
                {
                    _ServerRecv = value;
                    OnPropertyChanged(nameof(ServerRecv));
                }
            }
        }

        private string _ServerSend;
        public string ServerSend
        {
            get { return _ServerSend; }
            set
            {
                if (_ServerSend != value)
                {
                    _ServerSend = value;
                    OnPropertyChanged(nameof(ServerSend));
                }
            }
        }

        private int _ServerPort;
        public int ServerPort
        {
            get { return _ServerPort; }
            set
            {
                if (_ServerPort != value)
                {
                    _ServerPort = value;
                    OnPropertyChanged(nameof(ServerPort));
                }
            }
        }
        private string _LocalPort;
        public string LocalPort
        {
            get { return _LocalPort; }
            set
            {
                if (_LocalPort != value)
                {
                    _LocalPort = value;
                    OnPropertyChanged(nameof(LocalPort));
                }
            }
        }
        private string _ServerIp;
        public string ServerIp
        {
            get { return _ServerIp; }
            set
            {
                if (_ServerIp != value)
                {
                    _ServerIp = value;
                    OnPropertyChanged(nameof(ServerIp));
                }
            }
        }
        #endregion

        #region Tcp Client
        private string _ClientConnectBtnName;
        public string ClientConnectBtnName
        {
            get { return _ClientConnectBtnName; }
            set
            {
                if (_ClientConnectBtnName != value)
                {
                    _ClientConnectBtnName = value;
                    OnPropertyChanged(nameof(ClientConnectBtnName));
                }
            }

        }
        private string _ClientSendStr;
        public string ClientSendStr
        {
            get { return _ClientSendStr; }
            set
            {
                if (_ClientSendStr != value)
                {
                    _ClientSendStr = value;
                    OnPropertyChanged(nameof(ClientSendStr));
                }
            }
        }

        private int _ClientSendInterval;
        public int ClientSendInterval
        {
            get { return _ClientSendInterval; }
            set
            {
                if (_ClientSendInterval != value)
                {
                    _ClientSendInterval = value;
                    OnPropertyChanged(nameof(ClientSendInterval));
                }
            }
        }

        private string _ClientSendBtnName;
        public string ClientSendBtnName
        {
            get { return _ClientSendBtnName; }
            set
            {
                if (_ClientSendBtnName != value)
                {
                    _ClientSendBtnName = value;
                    OnPropertyChanged(nameof(ClientSendBtnName));
                }
            }
        }

        private string _ClientRecv;
        public string ClientRecv
        {
            get { return _ClientRecv; }
            set
            {
                if (_ClientRecv != value)
                {
                    _ClientRecv = value;
                    OnPropertyChanged(nameof(ClientRecv));
                }
            }
        }

        private string _ClientSend;
        public string ClientSend
        {
            get { return _ClientSend; }
            set
            {
                if (_ClientSend != value)
                {
                    _ClientSend = value;
                    OnPropertyChanged(nameof(ClientSend));
                }
            }
        }
        #endregion

        public TcpModel()
        {
            ListenPort = 65432;
            ServerSendStr = "Hello Client!";
            ServerSendInterval = 1000;
            ServerSend = "Hello Client!";
            ServerSendBtnName = "Auto Send Start";
            ServerListenBtnName = "Start Listen";

            ServerIp = "127.0.0.1";
            ServerPort = 65432;
            ClientSendStr = "Hello Server!";
            ClientSendInterval = 1000;
            ClientSend = "Hello Server!";
            ClientConnectBtnName = "Connect";
            ClientSendBtnName = "Auto Send Start";
            LocalPort = "0";
        }
    }
}
