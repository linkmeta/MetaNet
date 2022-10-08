using MetaNet.ViewModels;


namespace MetaNet.Models
{
    public class IperfModel : MainWindowBase
    {
        private string _Version;
        public string Version
        {
            get{ return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }
        private string _Role;
        public string Role
        {
            get { return _Role; }
            set
            {
                if (_Role != value)
                {
                    _Role = value;
                    OnPropertyChanged(nameof(Role));
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
        private int _Port;
        public int Port
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

        private int _Time;
        public int Time
        {
            get { return _Time; }
            set
            {
                if (_Time != value)
                {
                    _Time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }
        private int _Parallel;
        public int Parallel
        {
            get { return _Parallel; }
            set
            {
                if (_Parallel != value)
                {
                    _Parallel = value;
                    OnPropertyChanged(nameof(Parallel));
                }
            }
        }
        private int _Interval;
        public int Interval
        {
            get { return _Interval; }
            set
            {
                if (_Interval != value)
                {
                    _Interval = value;
                    OnPropertyChanged(nameof(Interval));
                }
            }
        }
        private int _TcpWindowSize;
        public int TcpWindowSize
        {
            get { return _TcpWindowSize; }
            set
            {
                if (_TcpWindowSize != value)
                {
                    _TcpWindowSize = value;
                    OnPropertyChanged(nameof(TcpWindowSize));
                }
            }
        }
        private string _TcpWindowUnit;
        public string TcpWindowUnit
        {
            get { return _TcpWindowUnit; }
            set
            {
                if (_TcpWindowUnit != value)
                {
                    _TcpWindowUnit = value;
                    OnPropertyChanged(nameof(TcpWindowUnit));
                }
            }
        }
        private int _BandWidth;
        public int BandWidth
        {
            get { return _BandWidth; }
            set
            {
                if (_BandWidth != value)
                {
                    _BandWidth = value;
                    OnPropertyChanged(nameof(BandWidth));
                }
            }
        }
        private string _BandWidthUnit;
        public string BandWidthUnit
        {
            get { return _BandWidthUnit; }
            set
            {
                if (_BandWidthUnit != value)
                {
                    _BandWidthUnit = value;
                    OnPropertyChanged(nameof(BandWidthUnit));
                }
            }
        }
        private int _PacketLen;
        public int PacketLen
        {
            get { return _PacketLen; }
            set
            {
                if (_PacketLen != value)
                {
                    _PacketLen = value;
                    OnPropertyChanged(nameof(PacketLen));
                }
            }
        }
        private bool _TcpFlag;
        public bool TcpFlag
        {
            get { return _TcpFlag; }
            set
            {
                if (_TcpFlag != value)
                {
                    _TcpFlag = value;
                    OnPropertyChanged(nameof(TcpFlag));
                }
            }
        }
        private bool _UdpFlag;
        public bool UdpFlag
        {
            get { return _UdpFlag; }
            set
            {
                if (_UdpFlag != value)
                {
                    _UdpFlag = value;
                    OnPropertyChanged(nameof(UdpFlag));
                }
            }
        }
        private bool _Reverse;
        public bool Reverse
        {
            get { return _Reverse; }
            set
            {
                if (_Reverse != value)
                {
                    _Reverse = value;
                    OnPropertyChanged(nameof(Reverse));
                }
            }
        }
        private double _Throughput;
        public double Throughput
        {
            get { return _Throughput; }
            set
            {
                if (_Throughput != value)
                {
                    _Throughput = value;
                    OnPropertyChanged(nameof(Throughput));
                }
            }
        }
        private string _Command;
        public string Command
        {
            get { return _Command; }
            set
            {
                if (_Command != value)
                {
                    _Command = value;
                    OnPropertyChanged(nameof(Command));
                }
            }
        }
        private string _Output;
        public string Output
        {
            get { return _Output; }
            set
            {
                if (_Output != value)
                {
                    _Output = value;
                    OnPropertyChanged(nameof(Output));
                }
            }
        }

        public IperfModel()
        {
            Version = "iperf.exe";
            Role = "-c";
            ServerIp = "10.21.68.29";
            Port = 5001;
            Parallel = 4;
            Time = 60;
            Interval = 1;
            TcpFlag = true;
            TcpWindowSize = 2;
            TcpWindowUnit = "M";
            UdpFlag = false;
            BandWidth = 100;
            BandWidthUnit = "M";
            PacketLen = 0;
            Reverse = false;
        }
    }
}
