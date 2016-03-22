using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using NetworkMonitor.Packets;
using System.Windows.Input;
using System.Windows.Threading;

namespace NetworkMonitor
{
    class MainViewModel
    {
        Dispatcher dispatcher;
        NetworkPacketsReceiver packetsReceiver;
        public ObservableCollection<PacketIP> Packets { get; set; }
        public ICommand MonitorStartCommand { get; set; }

        public MainViewModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            packetsReceiver = NetworkPacketsReceiver.Instance;
            Packets = new ObservableCollection<PacketIP>();
            packetsReceiver.PacketReceivedEvent += OnPacketReceived;
            MonitorStartCommand = new RelayCommand(arg => StartNetworkMonitor());
        }

        public async void StartNetworkMonitor ()
        {
            IPHostEntry ipHost = await Dns.GetHostEntryAsync(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[4];   // Дать пользователю выбрать IP в GUI;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            try
            {
                await Task.Run(() => packetsReceiver.Start(ipAddr, ipEndPoint));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK); }
        }

        void OnPacketReceived(PacketIP packet)
        {
            dispatcher.Invoke(new Action( ()=> Packets.Add(packet) ) );
        }
    }
}
