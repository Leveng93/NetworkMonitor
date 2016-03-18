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

namespace NetworkMonitor
{
    class MainViewModel
    {
        NetworkPacketsReceiver packetsReceiver;
        ObservableCollection<PacketIP> packets;

        public MainViewModel()
        {
            packetsReceiver = NetworkPacketsReceiver.Instance;
            packets = new ObservableCollection<PacketIP>();
            packetsReceiver.PacketReceivedEvent += OnPacketReceived;
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
            packets.Add(packet);
        }
    }
}
