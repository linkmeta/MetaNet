using MetaNet.ViewModels;

namespace MetaNet.Models
{
    public class NetworkScanModel : MainWindowBase
    {
        public class NetInterfaceInfo
        {
            public string name { get; set; }
            public string ip { get; set; }
            public string description { get; set; }
            public string mask { get; set; }
        }

        private NetInterfaceInfo[] _NetInfoItemSource;
        public NetInterfaceInfo[] NetInfoItemSource
        {
            get { return _NetInfoItemSource; }
            set
            {
                if (_NetInfoItemSource != value)
                {
                    _NetInfoItemSource = value;
                    OnPropertyChanged(nameof(NetInfoItemSource));
                }
            }
        }

        private string _StartIp;
        public string StartIp
        {
            get { return _StartIp; }
            set
            {
                if (_StartIp != value)
                {
                    _StartIp = value;
                    OnPropertyChanged(nameof(StartIp));
                }
            }
        }
        private string _StopIp;
        public string StopIp
        {
            get { return _StopIp; }
            set
            {
                if (_StopIp != value)
                {
                    _StopIp = value;
                    OnPropertyChanged(nameof(StopIp));
                }
            }
        }
        private string _IP;
        public string IP
        {
            get { return _IP; }
            set
            {
                if (_IP != value)
                {
                    _IP = value;
                    OnPropertyChanged(nameof(IP));
                }
            }
        }
        private int _OnlineCnt;
        public int OnlineCnt
        {
            get { return _OnlineCnt; }
            set
            {
                if (_OnlineCnt != value)
                {
                    _OnlineCnt = value;
                    OnPropertyChanged(nameof(OnlineCnt));
                }
            }
        }
        private int _OfflineCnt;
        public int OfflineCnt
        {
            get { return _OfflineCnt; }
            set
            {
                if (_OfflineCnt != value)
                {
                    _OfflineCnt = value;
                    OnPropertyChanged(nameof(OfflineCnt));
                }
            }
        }
        private string _ScanButtonName;
        public string ScanButtonName
        {
            get { return _ScanButtonName; }
            set
            {
                if (_ScanButtonName != value)
                {
                    _ScanButtonName = value;
                    OnPropertyChanged(nameof(ScanButtonName));
                }
            }
        }
        public NetworkScanModel()
        {
            StartIp = "192.168.1.1";
            StopIp = "192.168.1.255";
            ScanButtonName = "Start";
            OfflineCnt = 0;
            OnlineCnt = 0;
        }
    }
}
