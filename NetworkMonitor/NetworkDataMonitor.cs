using NetworkMonitor.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkMonitor
{
    /// <summary>
    /// Класс, предоставляющий данные о мониторинге сети. Синглтон.
    /// </summary>
    sealed class NetworkDataMonitor
    {
        public delegate void PackedRecived(PacketIP packet);
        public event PackedRecived PacketReceivedEvent;

        Socket mainSocket;  // Основной сокет.
        byte[] buffer;      // Буффер, в который считывается пакет.

        /// <summary>
        /// Отображение состояния сетевого монитора.
        /// </summary>
        public bool Started { get; private set; }

        private static readonly Lazy<NetworkDataMonitor> _instance = new Lazy<NetworkDataMonitor>(() => new NetworkDataMonitor());
        private NetworkDataMonitor() { }

        /// <summary>
        /// Предоставляет экземпляр NetworkDataMonitor.
        /// </summary>
        public static NetworkDataMonitor Instance { get { return _instance.Value; } }

        /// <summary>
        /// Запуск мониторинга в синхронном режиме.
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="ipEndPoint"></param>
        public void Start(IPAddress ipAddr, IPEndPoint ipEndPoint) 
        {
            if (Started) return;
            try
            {
                using (mainSocket = new Socket(ipAddr.AddressFamily, SocketType.Raw, ProtocolType.IP))   // Используется сырой сокет. Требуются права администратора.
                {
                    buffer = new byte[mainSocket.ReceiveBufferSize];    // Размер буффера равен размеру внутреннего буффера сокета.
                    mainSocket.Bind(ipEndPoint);
                    mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                    byte[] optionIn = new byte[4] { 1, 0, 0, 0 };
                    byte[] optionOut = new byte[4];
                    mainSocket.IOControl(IOControlCode.ReceiveAll, optionIn, optionOut);

                    Started = true;
                    while (Started)
                    {
                        int received = mainSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);  // Считываем пакет в буффер.
                        OnPacketReceivedEvent(new PacketIP(buffer, received));  // Создаем новый IP пакет, запускаем событие (рассылаем пакет подписчикам).
                        Array.Clear(buffer, 0, received); // Очищаем буффер.
                    }
                }
            }
            finally
            {
                Array.Clear(buffer, 0, buffer.Length);
                Started = false;
            }
        }

        // Запуск мониторинга в асинхронном режиме.
        public async Task StartAsync(IPAddress ipAddr, IPEndPoint ipEndPoint)
        {
            await Task.Run(() => Start(ipAddr, ipEndPoint));
        }

        public void Stop()
        {
            Started = false;  
        }

        /// <summary>
        /// Рассылка полученного пакета. 
        /// </summary>
        /// <param name="packet"></param>
        void OnPacketReceivedEvent (PacketIP packet)
        {
            PackedRecived pr;
            lock(this) { pr = PacketReceivedEvent; }
            if (pr != null) pr(packet);
        }
    }
}
