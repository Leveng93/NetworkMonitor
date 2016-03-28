using NetworkMonitor.Models.Packets;
using System.ComponentModel;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkMonitor.Models
{
    /// <summary>
    /// Класс, предоставляющий данные о мониторинге сети. Синглтон.
    /// </summary>
    class NetworkPacketsReceiver : INetworkPacketsReceiver, INotifyPropertyChanged
    {
        public event Action<PacketIP> PacketReceived = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        Socket _mainSocket;  // Основной сокет.
        byte[] _buffer;      // Буффер, в который считывается пакет.
        bool _started;

        /// <summary>
        /// Отображение состояние мониторинга.
        /// </summary>
        public bool Started {
            get { return _started; }
            private set
            {
                if (value != _started)
                {
                    _started = value;
                    OnPropertyChanged("Started");
                }
            }
        }

        private static readonly Lazy<NetworkPacketsReceiver> _instance = new Lazy<NetworkPacketsReceiver>(() => new NetworkPacketsReceiver());

        private NetworkPacketsReceiver() { }

        /// <summary>
        /// Предоставляет экземпляр NetworkDataMonitor.
        /// </summary>
        public static NetworkPacketsReceiver Instance { get { return _instance.Value; } }

        /// <summary>
        /// Событие изменения свойства.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        /// <summary>
        /// Запуск мониторинга в асинхронном режиме.
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="ipEndPoint"></param>
        public virtual async Task StartAsync(IPAddress ipAddr, IPEndPoint ipEndPoint) 
        {
            if (Started)
                throw new Exception("Start method is already running");
            
            try
            {
                using (_mainSocket = new Socket(ipAddr.AddressFamily, SocketType.Raw, ProtocolType.IP))   // Используется сырой сокет. Требуются права администратора.
                {
                    _buffer = new byte[_mainSocket.ReceiveBufferSize];    // Размер буффера равен размеру внутреннего буффера сокета.
                    _mainSocket.Bind(ipEndPoint);
                    _mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                    byte[] optionIn = new byte[4] { 1, 0, 0, 0 };
                    byte[] optionOut = new byte[4];
                    _mainSocket.IOControl(IOControlCode.ReceiveAll, optionIn, optionOut);

                    Started = true;
                    while (Started)
                    {
                        int received = await Task.Factory.FromAsync(_mainSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, null, null), _mainSocket.EndReceive);    // Считываем пакет в буффер.
                        PacketReceived(new PacketIP(_buffer, received));  // Создаем новый IP пакет, запускаем событие (рассылаем пакет подписчикам).
                        Array.Clear(_buffer, 0, received); // Очищаем буффер.
                    }
                }
            }
            finally
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                Started = false;
            }
        }

        /// <summary>
        /// Запуск мониторинга в синхронном режиме.
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="ipEndPoint"></param>
        public virtual void Start(IPAddress ipAddr, IPEndPoint ipEndPoint)
        {
            if (Started)
                throw new Exception("Start method is already running");

            try
            {
                using (_mainSocket = new Socket(ipAddr.AddressFamily, SocketType.Raw, ProtocolType.IP))   // Используется сырой сокет. Требуются права администратора.
                {
                    _buffer = new byte[_mainSocket.ReceiveBufferSize];    // Размер буффера равен размеру внутреннего буффера сокета.
                    _mainSocket.Bind(ipEndPoint);
                    _mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                    byte[] optionIn = new byte[4] { 1, 0, 0, 0 };
                    byte[] optionOut = new byte[4];
                    _mainSocket.IOControl(IOControlCode.ReceiveAll, optionIn, optionOut);

                    Started = true;
                    while (Started)
                    {
                        int received = _mainSocket.Receive(_buffer, 0, _buffer.Length, SocketFlags.None);    // Считываем пакет в буффер.
                        PacketReceived(new PacketIP(_buffer, received));  // Создаем новый IP пакет, запускаем событие (рассылаем пакет подписчикам).
                        Array.Clear(_buffer, 0, received); // Очищаем буффер.
                    }
                }
            }
            finally
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                Started = false;
            }
        }

        /// <summary>
        /// Остановка мониторинга. При принудительном завершении генерирует исключение ObjectDisposedException.
        /// </summary>
        public virtual void Stop(StopType stopType)
        {
            switch (stopType)
            {
                case StopType.Normal: Started = false; break; // Сокет будет закрыт после получения следующего пакета.
                case StopType.Immediately: if (Started) _mainSocket.Close(); break;   // Вызывает ObjectDisposedException.
            }
        }
    }
}
