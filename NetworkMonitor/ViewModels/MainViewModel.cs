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
        INetworkPacketsReceiver packetsReceiver;
        public int PacketsReceivedCount { get; private set; }
        public ObservableCollection<PacketIP> Packets { get; set; }
        public ICommand MonitorStart { get; set; }
        public ICommand MonitorStop { get; set; }
        public ICommand PacketCollectionClear { get; set; }
        
        public MainViewModel()
        {
            packetsReceiver = NetworkPacketsReceiver.Instance;
            Packets = new ObservableCollection<PacketIP>();
            packetsReceiver.PacketReceivedEvent += OnPacketReceived;
            MonitorStart = new RelayCommand(arg => StartNetworkMonitor());
            MonitorStop = new RelayCommand(arg => StopNetworkMonitor());
            PacketCollectionClear = new RelayCommand(arg => Packets.Clear());
        }

        public async void StartNetworkMonitor ()
        {
            IPHostEntry ipHost = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[4];   // Дать пользователю выбрать IP в GUI;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            try
            {
                await packetsReceiver.StartAsync(ipAddr, ipEndPoint);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK); }
        }

        public void StopNetworkMonitor ()
        {
            packetsReceiver.Stop();
        }

        void OnPacketReceived(PacketIP packet)
        {
            Packets.Add(packet);
            PacketsReceivedCount++;
        }
    }
}
