using MetaNet.Interfaces;
using MetaNet.Models;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static MetaNet.Models.PortModel;

namespace MetaNet.ViewModels
{
    internal class PortViewModel : MainWindowBase
    {
        public PortModel PortModel { get; set; }
        public ProcessInterface portProcess;


        public class PortListenStat
        {
            public string Protocol { get; set; }
            public string LocalAddress { get; set; }
            public string LocalPort { get; set; }
            public string RemoteAddress { get; set; }
            public string RemotePort { get; set; }
            public string Status { get; set; }
            public int Pid { get; set; }
            public string Program { get; set; }
        }
        public ObservableCollection<PortListenStat> PortListenStats { get; set; } = new ObservableCollection<PortListenStat>{ };

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
        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName + ".exe";
        }


        public void processPortInfo(string str)
        {
            string tcp_ipv4_pattern = @"(?<a>[A-Za-z]+)\s*(?<b>(\d+)\.(\d+)\.(\d+)\.(\d+))\:(?<c>(\d+))\s*(?<d>(\d+)\.(\d+)\.(\d+)\.(\d+))\:(?<e>(\d+))\s*(?<f>.* )\s*(?<g>\d+)";
            string tcp_ipv6_pattern = @"(?<a>[A-Za-z]+)\s*(?<b>\[.*\])\:(?<c>(\d+))\s*(?<d>\[.*\])\:(?<e>(\d+))\s*(?<f>[A-Za-z]+)\s*(?<g>\d+)";

            string udp_ipv4_pattern = @"(?<a>[A-Za-z]+)\s*(?<b>(\d+)\.(\d+)\.(\d+)\.(\d+))\:(?<c>(\d+))\s*(?<d>.* )\s*(?<e>(\d+))";
            string udp_ipv6_pattern = @"(?<a>[A-Za-z]+)\s*(?<b>\[.*\])\:(?<c>(\d+))\s*(?<d>.* )\s*(?<e>(\d+))";

            string protocol = "";
            string local = "";
            string localPort = "";
            string remote = "";
            string remotePort = "";
            string status = "";
            string program = "";
            int pid = 0;
            try
            {
                Match match = Match.Empty;
                if (str != null && str.Contains("TCP"))
                {
                    Match m = Regex.Match(str, tcp_ipv4_pattern);
                    Match n = Regex.Match(str, tcp_ipv6_pattern);

                    if (m.Success)
                    {
                        match = m;
                    }
                    else if (n.Success)
                    {
                        match = n;
                    }
                    remotePort = m.Groups["e"].Value;
                    status = m.Groups["f"].Value;
                    pid = int.Parse(match.Groups["g"].Value);
                    protocol = match.Groups["a"].Value;
                    local = match.Groups["b"].Value;
                    localPort = match.Groups["c"].Value;
                    remote = match.Groups["d"].Value;
                    program = LookupProcess(pid);
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        PortListenStats.Add(new PortListenStat { Protocol = protocol, LocalAddress = local, LocalPort = localPort, RemoteAddress = remote, RemotePort = remotePort, Status = status, Pid = pid, Program = program });
                    }));
                }
                else if (str != null && str.Contains("UDP"))
                {
                    Match s = Regex.Match(str, udp_ipv4_pattern);
                    Match t = Regex.Match(str, udp_ipv6_pattern);

                    if (s.Success)
                    {
                        match = s;
                    }
                    else if (t.Success)
                    {
                        match = t;
                    }
                    remotePort = "";
                    status = "";
                    pid = int.Parse(match.Groups["e"].Value);
                    protocol = match.Groups["a"].Value;
                    local = match.Groups["b"].Value;
                    localPort = match.Groups["c"].Value;
                    remote = match.Groups["d"].Value;
                    program = LookupProcess(pid);
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        PortListenStats.Add(new PortListenStat { Protocol = protocol, LocalAddress = local, LocalPort = localPort, RemoteAddress = remote, RemotePort = remotePort, Status = status, Pid = pid, Program = program });
                    }));
                }

            }
            catch (Exception e) {
            
            }
        }
        private void processOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            processPortInfo(e.Data);
            Console.WriteLine(e.Data);

        }
        public void GetPortInfo()
        {
            string routeCmd = "cmd.exe";

            portProcess.StartProcess(routeCmd, "/c netstat -aon", processOutputDataReceived);
        }
        public void RunPortCmd(object parameter)
        {
            PortListenStats.Clear();
            GetPortInfo();
        }
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(param => RunPortCmd(param));
            }
        }
        public PortViewModel()
        {
            PortModel = new PortModel();
            _IpInterface = new IpInterface();
            portProcess = new ProcessInterface();
            GetPortInfo();

            PortModel.NetInfoItemSource = GetLocalNetworkInterface();

        }
    }
}
