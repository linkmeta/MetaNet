using MetaNet.ViewModels;

namespace MetaNet.Models
{
    public class PortScanModel : MainWindowBase
    {
        private int _StartPort;
        public int StartPort
        {
            get { return _StartPort; }
            set
            {
                if (_StartPort != value)
                {
                    _StartPort = value;
                    OnPropertyChanged(nameof(StartPort));
                }
            }
        }
        private int _StopPort;
        public int StopPort
        {
            get { return _StopPort; }
            set
            {
                if (_StopPort != value)
                {
                    _StopPort = value;
                    OnPropertyChanged(nameof(StopPort));
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
        private string _Port;
        public string Port
        {
            get { return _Port; }
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }
        private int _OpenCnt;
        public int OpenCnt
        {
            get { return _OpenCnt; }
            set
            {
                if (_OpenCnt != value)
                {
                    _OpenCnt = value;
                    OnPropertyChanged(nameof(OpenCnt));
                }
            }
        }
        private int _CloseCnt;
        public int CloseCnt
        {
            get { return _CloseCnt; }
            set
            {
                if (_CloseCnt != value)
                {
                    _CloseCnt = value;
                    OnPropertyChanged(nameof(CloseCnt));
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
        private int _SocketTimeout;
        public int SocketTimeout
        {
            get { return _SocketTimeout; }
            set
            {
                if (_SocketTimeout != value)
                {
                    _SocketTimeout = value;
                    OnPropertyChanged(nameof(SocketTimeout));
                }
            }
        }
        public PortScanModel()
        {
            IP = "192.168.1.1";
            StartPort = 1;
            StopPort = 65535;
            SocketTimeout = 1000;
            ScanButtonName = "Start";
            CloseCnt = 0;
            OpenCnt = 0;
        }
    }
}
