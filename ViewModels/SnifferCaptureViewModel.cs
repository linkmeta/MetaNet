using MetaNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using PcapDotNet.Packets;
using PcapDotNet.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;

namespace MetaNet.ViewModels
{
    public class SnifferCaptureViewModel : MainWindowBase
    {

        ObservableCollection<Packet> packets;
        public ObservableCollection<Packet> Packets
        {
            get{ return packets; }
            private set{ packets = value; }
        }

        private string startCaptureBtnName;
        public string StartCaptureBtnName
        {
            get { return startCaptureBtnName; }
            set
            {
                if (startCaptureBtnName != value)
                {
                    startCaptureBtnName = value;
                    OnPropertyChanged(nameof(StartCaptureBtnName));
                }
            }
        }
        private bool saveStatus;
        public bool SaveStatus
        {
            get { return saveStatus; }
            set
            {
                if (saveStatus != value)
                {
                    saveStatus = value;
                    OnPropertyChanged(nameof(SaveStatus));
                }
            }
        }

        private List<IPacketDevice> networkInterfaceList;
        public List<IPacketDevice> NetworkInterfaceList
        {
            get { return networkInterfaceList; }
            set
            {
                if (networkInterfaceList != value)
                {
                    networkInterfaceList = value;
                    OnPropertyChanged(nameof(NetworkInterfaceList));
                }
            }
        }
        private IPacketDevice selectedInterface;
        public IPacketDevice SelectedInterface
        {
            get { return selectedInterface; }
            set
            {
                if (selectedInterface != value)
                {
                    selectedInterface = value;
                    OnPropertyChanged(nameof(SelectedInterface));
                }
            }
        }

        private Packet selectedPacket;
        public Packet SelectedPacket
        {
            get { return selectedPacket; }
            set
            {
                if (selectedPacket != value)
                {
                    selectedPacket = value;
                    OnPropertyChanged(nameof(SelectedPacket));
                }
            }
        }
        private string packetFilter;
        public string PacketFilter
        {
            get { return packetFilter; }
            set
            {
                if (packetFilter != value)
                {
                    packetFilter = value;
                    OnPropertyChanged(nameof(PacketFilter));
                }
            }
        }

        private CancellationTokenSource captureTokenSource;
        private PacketCommunicator communicator;

        public async void StartCaptureAsync(IPacketDevice packetDevice)
        {
            captureTokenSource = new CancellationTokenSource();
            Packet packet;
            try
            {
                await Task.Run(() =>
                {
                    communicator = packetDevice.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                    if (!string.IsNullOrEmpty(PacketFilter))
                    {
                        communicator.SetFilter(PacketFilter);
                    }

                    do
                    {
                        PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                        switch (result)
                        {
                            case PacketCommunicatorReceiveResult.Timeout:
                                // Timeout elapsed
                                continue;
                            case PacketCommunicatorReceiveResult.Ok:
                                //if (packet.Ethernet.EtherType == EthernetType.IpV4)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        packets.Add(packet);
                                        SnifferStatsProcess.UpdateStats(packet);
                                    });
                                }
                                break;
                            default:
                                throw new InvalidOperationException("PacketCommunicator InvalidOperationException");
                        }
                    } while (!captureTokenSource.Token.IsCancellationRequested);
                });
            }
            catch (Exception ex)
            {

            }
            finally
            {
                communicator.Break();
                communicator.Dispose();
                communicator = null;

                captureTokenSource.Dispose();
                captureTokenSource = null;
            }
        }

        public void StopCapture()
        {
            if(captureTokenSource != null)
                captureTokenSource.Cancel();
        }

        public ICommand StartCaptureCommand
        {
            get{ return new RelayCommand(param => StartCapture(param)); }
        }
        public void StartCapture(object parameter)
        {
            if (StartCaptureBtnName == "Start Capture")
            {
                StartCaptureBtnName = "Stop Capture";
                StartCaptureAsync(SelectedInterface);
                SnifferStatsProcess.Start();
                SaveStatus = false;
            }
            else
            {
                StartCaptureBtnName = "Start Capture";
                SnifferStatsProcess.Stop();
                StopCapture();
                SaveStatus = true;
            }
        }
        public ICommand FilterHelpCommand
        {
            get { return new RelayCommand(param => FilterHelp(param)); }
        }
        public void FilterHelp(object parameter)
        {
            var destinationurl = "https://www.tcpdump.org/manpages/pcap-filter.7.html";
            var sInfo = new ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            Process.Start(sInfo);
        }
        public ICommand SavePacketsCommand
        {
            get { return new RelayCommand(param => SavePackets(param)); }
        }
        public void SavePackets(object parameter)
        {
            SaveStatus = false;
            SaveFileDialog ReceDataSaveFileDialog = new SaveFileDialog
            {
                Title = "Save pcap packets",
                FileName = DateTime.Now.ToString("yyyyMMddmmss"),
                DefaultExt = ".pcap",
                Filter = "pcap files(*.pcap)|*.pcap|All files(*.*)|*.*"
            };

            if (ReceDataSaveFileDialog.ShowDialog() == true)
            {
                string DataRecvPath = ReceDataSaveFileDialog.FileName;
                Task.Run(() => {
                    foreach (var packet in packets)
                    {
                        PacketDumpFile.Dump(DataRecvPath, DataLinkKind.Ethernet, (int)SnifferStatsProcess.ByteCount, packets);
                    }
                    SaveStatus = true;
                });
            } 
        }
        private void GetNetworkInterfaceList()
        {
            NetworkInterfaceList = new List<IPacketDevice>();
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            for (int i = 0; i < allDevices.Count(); i++)
                NetworkInterfaceList.Add(allDevices[i]);

        }
        public SnifferCaptureViewModel()
        {
            Packets = new ObservableCollection<Packet>();
            StartCaptureBtnName = "Start Capture";

            GetNetworkInterfaceList();
        }
    }
}
