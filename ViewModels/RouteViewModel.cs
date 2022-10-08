using MetaNet.Interfaces;
using MetaNet.Models;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static MetaNet.Models.RouteModel;

namespace MetaNet.ViewModels
{
    internal class RouteViewModel : MainWindowBase
    {
        public RouteModel RouteModel { get; set; }
        public ProcessInterface routeProcess;

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(param => RunRoute(param));
            }
        }
        public class Ipv4Result
        {
            public string DestAddress { get; set; }
            public string Mask { get; set; }
            public string Gateway { get; set; }
            public string Interface { get; set; }
            public int Metric { get; set; }

        }
        public ObservableCollection<Ipv4Result> Ipv4Results { get; set; } = new ObservableCollection<Ipv4Result>
        {

        };
        public class Ipv6Result
        {
            public string Interface { get; set; }
            public int Metric { get; set; }
            public string DestAddress { get; set; }
            public string Gateway { get; set; }

        }
        public ObservableCollection<Ipv6Result> Ipv6Results { get; set; } = new ObservableCollection<Ipv6Result>
        {
        };
        public NetInterfaceInfo[] GetLocalNetworkInterface()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            NetInterfaceInfo[] info = new NetInterfaceInfo[len];
            //string[] name = new string[len];
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

        public IpInterface _IpInterface;
        public void processRouteInfo(string str)
        {
            string ipv6_pattern = @"(?<a>(\d+))\s*(?<b>(\d+))\s*(?<c>.* )\s*(?<d>.*)";
            string ipv4_pattern = @"(?<a>(\d+)\.(\d+)\.(\d+)\.(\d+))\s*(?<b>(\d+)\.(\d+)\.(\d+)\.(\d+))\s*(?<c>.* )\s*(?<d>(\d+)\.(\d+)\.(\d+)\.(\d+))\s*(?<e>\d+)";

            if(str != null && !str.Contains("..."))
            {
                Match m = Regex.Match(str, ipv4_pattern);
                if (m.Success)
                {
                    string dest = m.Groups["a"].Value;
                    string mask = m.Groups["b"].Value;
                    string gateway = m.Groups["c"].Value;
                    string interfaces = m.Groups["d"].Value;
                    string metric = m.Groups["e"].Value;

                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        Ipv4Results.Add(new Ipv4Result { DestAddress = dest, Mask = mask, Gateway = gateway.Trim(), Interface = interfaces, Metric = Convert.ToInt32(metric) });
                    }));
                }
                else
                {

                }
                if (str.Contains(":"))
                {
                    Match n = Regex.Match(str, ipv6_pattern);

                    if (n.Success)
                    {
                        string interfaces = n.Groups["a"].Value;
                        string metric = n.Groups["b"].Value;
                        string dest = n.Groups["c"].Value;
                        string gateway = n.Groups["d"].Value;
                        if(dest == " " && gateway != "")
                        {
                            dest = gateway;
                            gateway = "";
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            Ipv6Results.Add(new Ipv6Result { Interface = interfaces, Metric = Convert.ToInt32(metric), DestAddress = dest.Trim(),  Gateway = gateway});
                        }));
                    }
                    else
                    {

                    }
                }
            }
        }
        private void processOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            processRouteInfo(e.Data);
            Console.WriteLine(e.Data);
        }
        public void GetRouteInfo()
        {
            string routeCmd = "cmd.exe";

            routeProcess.StartProcess(routeCmd, "/c route print", processOutputDataReceived);
        }
        public void RunRoute(object parameter)
        {
            Ipv4Results.Clear();
            Ipv6Results.Clear();
            GetRouteInfo();
        }

        public RouteViewModel()
        {
            RouteModel = new RouteModel();
            _IpInterface = new IpInterface();
            routeProcess = new ProcessInterface();
            GetRouteInfo();

            RouteModel.NetInfoItemSource = GetLocalNetworkInterface();

        }
    }
}
