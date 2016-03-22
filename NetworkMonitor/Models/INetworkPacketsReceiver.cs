using System;
using System.Net;
using System.Threading.Tasks;
using NetworkMonitor.Models.Packets;

namespace NetworkMonitor.Models
{
    interface INetworkPacketsReceiver
    {
        bool Started { get; }   // Состояние.

        event Action<PacketIP> PacketReceivedEvent; // Событие, запускаемое при получении нового пакета.

        Task StartAsync(IPAddress ipAddr, IPEndPoint ipEndPoint);
        void Stop();
    }
}