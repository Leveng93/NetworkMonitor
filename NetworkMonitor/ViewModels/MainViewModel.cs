using NetworkMonitor.Models;
using NetworkMonitor.Models.Packets;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace NetworkMonitor.ViewModels
{
    class MainViewModel
    {
        #region FieldsAndProps

        INetworkPacketsReceiver _packetsReceiver;
        public ObservableCollection<PacketIP> Packets { get; set; }

        #endregion // FieldsAndProps

        #region Commands

        RelayCommand _monitorStart;
        public ICommand MonitorStart
        {
            get
            {
                if (_monitorStart == null)
                    _monitorStart = new RelayCommand(arg => StartNetworkMonitor());
                
                return _monitorStart;
            }
        }

        RelayCommand _monitorStop;
        public ICommand MonitorStop
        {
            get
            {
                if (_monitorStop == null)
                    _monitorStop = new RelayCommand(arg => StopNetworkMonitor());
                return _monitorStop;
            }
        }

        RelayCommand _clearPacketCollection;
        public ICommand ClearPacketCollection
        {
            get
            {
                if (_clearPacketCollection == null)
                    _clearPacketCollection = new RelayCommand(arg => Packets.Clear());
                return _clearPacketCollection;
            }
        }

        #endregion // Commands

        #region Constructors

        public MainViewModel()
        {
            _packetsReceiver = NetworkPacketsReceiver.Instance;
            Packets = new ObservableCollection<PacketIP>();
            _packetsReceiver.PacketReceivedEvent += OnPacketReceived;
        }

        #endregion // Constructors

        #region Methods

        public async void StartNetworkMonitor ()
        {
            IPHostEntry ipHost = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[4];   // Дать пользователю выбрать IP в GUI;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            try
            {
                await _packetsReceiver.StartAsync(ipAddr, ipEndPoint);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK); }
        }

        public void StopNetworkMonitor ()
        {
            _packetsReceiver.Stop();
        }

        void OnPacketReceived(PacketIP packet)
        {
            Packets.Add(packet);

        }

        #endregion // Methods
    }
}
