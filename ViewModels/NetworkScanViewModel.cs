using MetaNet.Interfaces;
using MetaNet.Models;
using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static MetaNet.Models.NetworkScanModel;

namespace MetaNet.ViewModels
{
    internal class NetworkScanViewModel : MainWindowBase
    {

        public NetworkScanModel NetworkScanModel { get; set; }

        public IpInterface _IpInterface;

        public int ipCnt = 0;
        internal CancellationTokenSource scanTokenSource;
        internal CancellationToken cancelScanToken;

        public class IpScanResult
        {
            public string IpAddress { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public string Time { get; set; }
            public string Ttl { get; set; }

        }
        public ObservableCollection<IpScanResult> IpScanResults { get; set; } = new ObservableCollection<IpScanResult> { };
        private void DoScan(string ip )
        {
            PingReply reply = null;
            string status = null;
            string hostname = null;
            string time = null;
            string ttl = null;
            Console.WriteLine(ip);
            reply = _IpInterface.PingSweep(ip);

            NetworkScanModel.IP = ip;
            if (reply != null)
            {
                if (reply.Status == IPStatus.Success)
                {
                    status = "Online";
                    hostname = _IpInterface.GetHostName(ip);
                    time = reply.RoundtripTime.ToString();
                    ttl = reply.Options.Ttl.ToString();
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IpScanResults.Add(new IpScanResult { IpAddress = ip, Name = hostname, Status = status, Time = time, Ttl = ttl });
                    }));
                    NetworkScanModel.OnlineCnt++;
                }
                else
                {
                    status = "Timeout";
                    NetworkScanModel.OfflineCnt++;
                }
            }
            else
            {
                status = "Offline";
                NetworkScanModel.OfflineCnt++;
            }
            ipCnt--;
            Console.WriteLine(ipCnt.ToString());
            if (ipCnt == 0)
            {
                NetworkScanModel.ScanButtonName = "Start";
            }
        }

        public async void StartScanAsync()
        {
            scanTokenSource = new CancellationTokenSource();
            cancelScanToken = scanTokenSource.Token;
            uint startIpVal = _IpInterface.IPAddressToLongBackwards(NetworkScanModel.StartIp);
            string startIpStr = _IpInterface.LongToIPAddress(startIpVal);

            uint endIpVal = _IpInterface.IPAddressToLongBackwards(NetworkScanModel.StopIp);
            string endIpStr = _IpInterface.LongToIPAddress(endIpVal);
            ipCnt = (int)(endIpVal - startIpVal);
            if(ipCnt <= 0)
            {
                MessageBox.Show("Please make sure (Start Ip) < (Stop Ip)!");
                return;
            }
            try
            {
                await Task.Run(() =>
                {
                    for (uint i = startIpVal; i <= endIpVal; i++)
                    {
                        Console.WriteLine(i.ToString());

                        var j = i;
                        var ipStr = _IpInterface.LongToIPAddress(j);
                        var task = Task.Run(() =>
                        {
                            DoScan(ipStr);
                        }, cancelScanToken);
                        Thread.Sleep(1);
                    }
                }, cancelScanToken);
            }
            catch (Exception ex)
            {

            }
        }
        internal void StopScanTask()
        {
            scanTokenSource.Cancel();
        }

        public void IpScanCmd(object parameter)
        {
            if(NetworkScanModel.ScanButtonName == "Start")
            {
                NetworkScanModel.ScanButtonName = "Stop";
                NetworkScanModel.OfflineCnt = 0;
                NetworkScanModel.OnlineCnt = 0;
                IpScanResults.Clear();
                //Do Scan
                StartScanAsync();

            }
            else
            {
                //Do Stop
                StopScanTask();
                NetworkScanModel.ScanButtonName = "Start";
            }
        }
        public ICommand IpScanCommand
        {
            get
            {
                return new RelayCommand(param => IpScanCmd(param));
            }
        }
        public NetInterfaceInfo[] GetLocalNetworkInterface()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;
            NetInterfaceInfo[] info = new NetInterfaceInfo[len];

            int j = 0;
            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
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
                            info[j] = new NetInterfaceInfo();
                            info[j].description = ni.Description;
                            info[j].name = niname;
                            info[j].ip = address;
                            info[j].mask = mask;
                            j++;
                        }
                    }
                }
            }
            return info;
        }
  
        public NetworkScanViewModel()
        {
            NetworkScanModel = new NetworkScanModel();
            _IpInterface = new IpInterface();

            NetworkScanModel.NetInfoItemSource = GetLocalNetworkInterface();

        }
    }
}
