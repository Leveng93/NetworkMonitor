using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace NetworkMonitor.Models.Packets
{
    /// <summary>
    /// IP пакет, содержащий в себе заголовок и данные.
    /// </summary>
    class PacketIP : IGroupedData<string>
    {
        #region Fields

        Byte _versionAndHeaderLength;    // Версия протокола IP (первые 4 бита) + длина заголовка (следующие 4 бита).
        Byte _serviceType;               // Тип сервиса. Занимает 1 байт и задает приоритет дейтаграммы и желаемый тип маршрутизации.
        UInt16 _totalLen;                // Общая длина. Занимает 2 байта и указывает общую длину пакета с учетом заголовка и поля данных.
        UInt16 _id;                      // Идентификатор пакета. Занимает 2 байта и используется для распознавания пакетов, образовавшихся путем фрагментации исходного пакета.
        UInt16 _flagsAndOffset;          // Первые 3 бита - флаги фрагментации. Следующие 13 - смещение поля данных этого пакета от начала общего поля данных исходного пакета, подвергнутого фрагментации.
        Byte _ttl;                       // Время жизни пакета.
        Byte _protocol;                  // Протокол верхнего уровня. (TCP, UDP или RIP).
        Int16 _checksum;                 // Контрольная сумма заголовка. Занимает 2 байта, рассчитывается по всему заголовку.
        UInt32 _sourceIP;                // Адрес источника. (4 байта).
        UInt32 _destIP;                  // Адрес получателя. (4 байта).

        Byte[] _data;                    // Сообщение, содержащееся после заголовка.

        Byte _headerLength;              // Длина заголовка.
        UInt16 _messageLength;           // Длина сообщения.

        List<string> _groupedData;

        #endregion // Fields

        #region Constructors

        PacketIP() { }

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketIP.
        /// </summary>
        /// <param name="Buffer">Массив байт для парсинга</param>
        /// <param name="Received">Количество байт в массиве</param>
        public PacketIP(Byte[] Buffer, Int32 Received)
        {
            using (MemoryStream memoryStream = new MemoryStream(Buffer, 0, Received))
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                _versionAndHeaderLength = binaryReader.ReadByte();
                _serviceType = binaryReader.ReadByte();
                _totalLen = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                _id = (UInt16)IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                _flagsAndOffset = (UInt16)IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                _ttl = binaryReader.ReadByte();
                _protocol = binaryReader.ReadByte();
                _checksum = IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                _sourceIP = (UInt32)binaryReader.ReadInt32();
                _destIP = (UInt32)binaryReader.ReadInt32();

                _headerLength = _versionAndHeaderLength;
                _headerLength <<= 4;
                _headerLength >>= 4;
                _headerLength *= 4;  // Т.к. поле headerLength содержит в себе количество 32х-битных слов, домножаем на 4, чтобы получить количество байт.

                _messageLength = (UInt16)(_totalLen - _headerLength);

                _data = new byte[_messageLength];
                Array.Copy(Buffer, _headerLength, _data, 0, _data.Length);
            }
        }

        #endregion // Constructors

        #region Properties
        /// <summary>
        /// Версия протокола.
        /// </summary>
        public String Version
        {
            get
            {
                Byte version = (Byte)(_versionAndHeaderLength >> 4);
                if (version == 4) return "IPv4";
                else if (version == 6) return "IPv6";
                else return "Unknown";
            }
        }

        /// <summary>
        /// Длина IP заголовка.
        /// </summary>
        public Byte HeaderLength
        {
            get { return _headerLength; }
        }

        /// <summary>
        /// Тип сервиса. Задает приоритет дейтаграммы и желаемый тип маршрутизации.
        /// </summary>
        public String ServiceType
        {
            get { return String.Format("0x{0:x2} ({0})", _serviceType); }
        }

        /// <summary>
        /// Общая длина всего IP пакета.
        /// </summary>
        public UInt16 TotalLen
        {
            get { return _totalLen; }
        }

        /// <summary>
        /// Идентификатор пакета. Используется для распознавания пакетов после фрагментации.
        /// </summary>
        public UInt16 Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Флаги фрагментации.
        /// </summary>
        public String Flags
        {
            get
            {
                UInt16 flags = (UInt16)(_flagsAndOffset >> 13);
                if (flags == 2) return "Not fragmented";
                else if (flags == 1) return "Fragmented";
                return flags.ToString();
            }
        }

        /// <summary>
        /// Смещение фрагментации.
        /// </summary>
        public UInt16 Offset
        {
            get
            {
                UInt16 offset = (UInt16)(_flagsAndOffset << 3);
                offset >>= 3;
                return offset;
            }
        }

        /// <summary>
        /// Время жизни пакета.
        /// </summary>
        public Byte TTL
        {
            get { return _ttl; }
        }

        public String Protocol
        {
            /*
                1: ICMP - Протокол контрольных сообщений.
                2: IGMP - Протокол управления группой хостов.
                6: TCP.
                17: UDP.
            */
            get
            {
                switch (_protocol)
                {
                    case 6: return "TCP";
                    case 17: return "UDP";
                    case 1: return "ICMP";
                    case 2: return "IGMP";
                    default: return "Unknown";
                }
            }
        }

        /// <summary>
        /// Контрольная сумма заголовка.
        /// </summary>
        public String Checksum
        {
            get { return "0x" + _checksum.ToString("x"); }
        }

        /// <summary>
        /// Адрес отправителя.
        /// </summary>
        public IPAddress SourceIP
        {
            get { return new IPAddress(_sourceIP); }
        }

        /// <summary>
        /// Адрес получателя.
        /// </summary>
        public IPAddress DestinationIP
        {
            get { return new IPAddress(_destIP); }
        }

        /// <summary>
        /// Длина сообщения из IP пакета.
        /// </summary>
        public UInt16 MessageLength
        {
            get { return _messageLength; }
        }

        /// <summary>
        /// Сообщение из IP пакета.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
        }

        #endregion // Properties

        public IEnumerable<string> GetGroupedData()
        {
            if (_groupedData != null) return _groupedData.AsReadOnly();

            _groupedData = new List<string>();

            _groupedData.Add("Version: " + Version);
            _groupedData.Add("Header length: " + HeaderLength.ToString());
            _groupedData.Add("Service type: " + ServiceType);
            _groupedData.Add("Total length: " + TotalLen);
            _groupedData.Add("ID: " + Id);
            _groupedData.Add("Fragmentation flags: " + Flags);
            _groupedData.Add("Fragmentation offset: " + Offset);
            _groupedData.Add("TTL: " + TTL);
            _groupedData.Add("Up level protocol: " + Protocol);
            _groupedData.Add("Check sum: " + Checksum);
            _groupedData.Add("Source IP: " + SourceIP);
            _groupedData.Add("Destination IP: " + DestinationIP);
            _groupedData.Add("Message length: " + MessageLength);

            return _groupedData.AsReadOnly();
        }
    }
}