using MetaNet.ViewModels;

namespace MetaNet.Models
{
    public class ServerModel : MainWindowBase
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

        public ServerModel()
        {
        }
    }
}
