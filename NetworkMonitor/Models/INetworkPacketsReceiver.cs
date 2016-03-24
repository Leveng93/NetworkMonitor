using System;
using System.Net;
using System.Threading.Tasks;
using NetworkMonitor.Models.Packets;

namespace NetworkMonitor.Models
{
    enum StopType {
        /// <summary>
        /// Нормальное завершение мониторинга.
        /// </summary>
        Normal,
        /// <summary>
        /// Принудительное завершение мониторинга с исключением.
        /// </summary>
        Immediately
    }

    /// <summary>
    /// Получает IP пакеты.
    /// </summary>
    interface INetworkPacketsReceiver
    {
        bool Started { get; }   // Состояние.

        event Action<PacketIP> PacketReceivedEvent; // Событие, запускаемое при получении нового пакета.

        void Start(IPAddress ipAddr, IPEndPoint ipEndPoint);        // Запуск мониторинга в синхронном режиме.
        Task StartAsync(IPAddress ipAddr, IPEndPoint ipEndPoint);   // Запуск мониторинга в асинхроном режиме.
        void Stop(StopType stopType);    // Остановка мониторинга.
    }
}