using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NetworkMonitor.Packets;

namespace NetworkMonitor
{
    /// <summary>
    /// Класс, предоставляющий данные о мониторинге сети. Синглтон.
    /// </summary>
    sealed class NetworkDataMonitor
    {
        public delegate void PackedRecived(PacketIP packet);
        public event PackedRecived PacketReceivedEvent;

        Socket mainSocket;
        byte[] buffer;

        public bool Started { get; private set; }

        private static readonly Lazy<NetworkDataMonitor> _instance = new Lazy<NetworkDataMonitor>(() => new NetworkDataMonitor());
        private NetworkDataMonitor() { }

        /// <summary>
        /// Предоставляет экземпляр NetworkDataMonitor.
        /// </summary>
        public static NetworkDataMonitor Instance { get { return _instance.Value; } }

        public void Start(IPAddress ipAddr, IPEndPoint ipEndPoint)   // Запускает мониторинг в синхронном режиме. 
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
                        Array.Clear(buffer, 0, received); // Очищаем пакет.
                    }
                }
            }
            finally
            {
                Array.Clear(buffer, 0, buffer.Length);
                Started = false;
            }
        }

        public async Task StartAsync(IPAddress ipAddr, IPEndPoint ipEndPoint)
        {
            await Task.Run(() => Start(ipAddr, ipEndPoint));
        }


        public void Stop()
        {
            Started = false;  
        }

        void OnPacketReceivedEvent (PacketIP packet)
        {
            PackedRecived pr;
            lock(this) { pr = PacketReceivedEvent; }
            if (pr != null) pr(packet);
        }
    }
}
