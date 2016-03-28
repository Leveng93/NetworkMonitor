using NetworkMonitor.Models;
using NetworkMonitor.Models.Packets;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace NetworkMonitor.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region FieldsAndProps

        public INetworkPacketsReceiver PacketsReceiver { get; private set; }
        public ObservableCollection<PacketInfo> Packets { get; private set; }

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

        PacketInfo _selectedValue;
        public PacketInfo SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (value != null && value != _selectedValue)
                {
                    _selectedValue = value;
                    ShowPackageDetails(_selectedValue);
                }                   
                OnPropertyChanged("SelectedValue");
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
        }

        #endregion // Constructors

        #region Methods

        async void StartNetworkMonitor ()
        {
            IPHostEntry ipHost = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[4];   // Дать пользователю выбрать IP в GUI;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            try
            {
                await PacketsReceiver.StartAsync(ipAddr, ipEndPoint);
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

        void ShowPackageDetails (PacketInfo packet)
        {
            MessageBox.Show(packet.PacketNumber.ToString());
        }

        #endregion // Methods
    }
}
