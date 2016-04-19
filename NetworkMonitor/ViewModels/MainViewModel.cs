using NetworkMonitor.Models;
using NetworkMonitor.Models.Packets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace NetworkMonitor.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region FieldsAndProps

        public INetworkPacketsReceiver PacketsReceiver { get; private set; }
        public ObservableCollection<PacketInfo> Packets { get; private set; }
        public ObservableCollection<IPAddress> IpAddresses { get; private set; }

        ulong _packetsReceivedCount;
        public ulong PacketsReceivedCount
        {
            get { return _packetsReceivedCount; }
            set
            {
                _packetsReceivedCount = value;
                OnPropertyChanged("PacketsReceivedCount");
            }
        }

        PacketInfo _selectedPacket;
        public PacketInfo SelectedPacket
        {
            get { return _selectedPacket; }
            set
            {
                if (value != null && value != _selectedPacket)
                    _selectedPacket = value;                                 
                OnPropertyChanged("SelectedPacket");
            }
        }

        IPAddress _selectedIp;
        public IPAddress SelectedIP
        {
            get { return _selectedIp; }
            set
            {
                if (value != null && value != _selectedIp)
                    _selectedIp = value;
                OnPropertyChanged("_selectedIp");
            }
        }

        #endregion // FieldsAndProps

        #region Commands

        public ICommand MonitorStart { get; private set; }
        public ICommand MonitorStop { get; private set; }
        public ICommand ClearPacketCollection { get; private set; }

        #endregion // Commands

        #region Constructors

        public MainViewModel()
        {
            PacketsReceiver = NetworkPacketsReceiver.Instance;
            PacketsReceiver.PacketReceived += OnPacketReceived;
            Packets = new ObservableCollection<PacketInfo>();

            MonitorStart = new RelayCommand(arg => StartNetworkMonitor());
            MonitorStop = new RelayCommand(arg => StopNetworkMonitor());
            ClearPacketCollection = new RelayCommand(arg => Packets.Clear());

            GetIpAddressListAsync();
        }

        #endregion // Constructors

        #region Methods

        async void GetIpAddressListAsync()
        {
            try
            {
                IPHostEntry ipHost;
                await Task.Run(async () =>
                {
                    ipHost = await Dns.GetHostEntryAsync(Dns.GetHostName());
                    List<IPAddress> tempIpAdd = new List<IPAddress>();
                    foreach (var address in ipHost.AddressList)
                        if (address.AddressFamily == AddressFamily.InterNetwork)
                            tempIpAdd.Add(address);
                    IpAddresses = new ObservableCollection<IPAddress>(tempIpAdd);
                    if (IpAddresses.Last() != null)
                        SelectedIP = IpAddresses.Last();
                });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK); }
        }

        async void StartNetworkMonitor ()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(SelectedIP, 11000);
                await PacketsReceiver.StartAsync(SelectedIP, ipEndPoint);
            }
            catch (ObjectDisposedException) { } // Возникает при принудительном закрытии сокета методом Close().
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK); }
        }

        void StopNetworkMonitor ()
        {
            PacketsReceiver.Stop(StopType.Immediately);
        }

        void OnPacketReceived(PacketIP packet)
        {
            Packets.Add(new PacketInfo(packet, PacketsReceivedCount, DateTime.Now));
            PacketsReceivedCount++;
        }

        #endregion // Methods
    }
}
