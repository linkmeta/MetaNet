using MetaNet.Models;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using static MetaNet.Models.ServerModel;


namespace MetaNet.ViewModels
{
    internal class ServerViewModel : MainWindowBase
    {
        public ServerModel ServerModel { get; set; }
        public NetInterfaceInfo[] GetLocalNetworkInterface()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            NetInterfaceInfo[] info = new NetInterfaceInfo[len];

            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
                info[i] = new NetInterfaceInfo();
                info[i].description = ni.Description;

                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties property = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in property.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string address = ip.Address.ToString();
                            string niname = ni.Name.ToString();
                            string mask = ip.IPv4Mask.ToString();
                            info[i].name = niname;
                            info[i].ip = address;
                            info[i].mask = mask;
                        }
                    }
                }
            }
            return info;
        }

        ObservableCollection<object> _ServerViews;
        public ObservableCollection<object> ServerViews { get { return _ServerViews; } }
        public ServerViewModel()
        {
            ServerModel = new ServerModel();

            _ServerViews = new ObservableCollection<object>();
            _ServerViews.Add(new TcpViewModel());
            _ServerViews.Add(new UdpViewModel());
            _ServerViews.Add(new WebSocketViewModel());
 
            ServerModel.NetInfoItemSource = GetLocalNetworkInterface();
        }

 
    }
}
