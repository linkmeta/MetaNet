using MetaNet.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MetaNet.ViewModels
{
    internal class MainWindowVM : MainWindowBase
    {
        // CollectionViewSource enables XAML code to set the commonly used CollectionView properties,
        // passing these settings to the underlying view.
        private CollectionViewSource MenuItemsCollection;

        // ICollectionView enables collections to have the functionalities of current record management,
        // custom sorting, filtering, and grouping.
        public ICollectionView SourceCollection => MenuItemsCollection.View;


        private string filterText;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                MenuItemsCollection.View.Refresh();
                OnPropertyChanged("FilterText");
            }
        }
        private void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            MenuItems _item = e.Item as MenuItems;
            if (_item.MenuName.ToUpper().Contains(FilterText.ToUpper()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public void RunCalc(object parameter)
        {
            Process.Start("calc.exe");

        }
        public ICommand RunCalcCommand
        {
            get
            {
                return new RelayCommand(param => RunCalc(param));
            }
        }
        public void RunMenuCmd(object parameter)
        { 
            switch (parameter)
            {
                case "Calc":
                    Process.Start("calc.exe");
                    break;
                case "About":
                    AboutWindow aboutWindow = new AboutWindow();
                    aboutWindow.Show();
                    Dispatcher.Run();
                    break;
                case "Iperf":
                    var iperfInfo = new ProcessStartInfo("https://iperf.fr/")
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(iperfInfo);
                    break;

                case "Pcap":
                    var pcapInfo = new ProcessStartInfo("https://www.tcpdump.org/manpages/pcap-filter.7.html")
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(pcapInfo);
                    break;

                default:
                    break;
            }
        }
        public ICommand RunMenuCommand
        {
            get
            {
                return new RelayCommand(param => RunMenuCmd(param));
            }
        }

        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set { _selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }
        
        
        // Switch Views

        // Menu Button Command
        private ICommand _menucommand;
        public ICommand MenuCommand
        {
            get
            {
                if (_menucommand == null)
                {
                    _menucommand = new RelayCommand(param => SwitchViews(param));
                }
                return _menucommand;
            }
        }
        public class MenuItems
        {
            public string MenuName { get; set; }
            public string MenuImage { get; set; }
        }
        public IperfViewModel iperfVM = null;
        public NetworkScanViewModel networkScanVM = null;
        public PortScanViewModel portScanVM = null;
        public RouteViewModel routeVM = null;
        public PortViewModel portVM = null;
        public ServerViewModel serverVM = null;
        public SnifferViewModel snifferVM = null;
        public void SwitchViews(object parameter)
        {
            switch (parameter)
            {
                case "Iperf":
                    if (iperfVM == null)
                    {
                        iperfVM = new IperfViewModel();
                    }
                    SelectedViewModel = iperfVM;// new IperfViewModel();
                    break;
                case "NetworkScan":
                    if (networkScanVM == null)
                    {
                        networkScanVM = new NetworkScanViewModel();
                    }
                    SelectedViewModel = networkScanVM;// new NetworkScanViewModel();
                    break;
                case "PortScan":
                    if (portScanVM == null)
                    {
                        portScanVM = new PortScanViewModel();
                    }
                    SelectedViewModel = portScanVM;
                    break;
                case "RouteTable":
                    if (routeVM == null)
                    {
                        routeVM = new RouteViewModel();
                    }
                    SelectedViewModel = routeVM;
                    break;

                case "PortListen":
                    if (portVM == null)
                    {
                        portVM = new PortViewModel();
                    }
                    SelectedViewModel = portVM;
                    break;
                case "Server":
                    if (serverVM == null)
                    {
                        serverVM = new ServerViewModel();
                    }
                    SelectedViewModel = serverVM;
                    break;
                case "Sniffer":
                    if (snifferVM == null)
                    {
                        snifferVM = new SnifferViewModel();
                    }
                    SelectedViewModel = snifferVM;
                    break;
                default:
                    break;
            }
        }
        public MainWindowVM()
        {
            ObservableCollection<MenuItems> menuItems = new ObservableCollection<MenuItems>
            {
                new MenuItems { MenuName = "Iperf", MenuImage = @"Resources/Speed.png" },
                new MenuItems { MenuName = "NetworkScan", MenuImage = @"Resources/IP.png" },
                new MenuItems { MenuName = "PortScan", MenuImage = @"Resources/port.png" },
                new MenuItems { MenuName = "RouteTable", MenuImage = @"Resources/route.png" },
                new MenuItems { MenuName = "PortListen", MenuImage = @"Resources/portlisten.png" },
                new MenuItems { MenuName = "Server", MenuImage = @"Resources/servers.png" },
                new MenuItems { MenuName = "Sniffer", MenuImage = @"Resources/sniffer.png" }
            };

            MenuItemsCollection = new CollectionViewSource { Source = menuItems };
            MenuItemsCollection.Filter += MenuItems_Filter;

            iperfVM = new IperfViewModel();
            SelectedViewModel = iperfVM;
        }
    }
}
