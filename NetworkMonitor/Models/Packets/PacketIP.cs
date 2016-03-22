using System;
using System.Net;
using System.IO;

namespace NetworkMonitor.Models.Packets
{
    /// <summary>
    /// IP пакет, содержащий в себе заголовок и данные.
    /// </summary>
    class PacketIP
    {
        Byte versionAndHeaderLength;    // Версия протокола IP (первые 4 бита) + длина заголовка (следующие 4 бита).
        Byte serviceType;               // Тип сервиса. Занимает 1 байт и задает приоритет дейтаграммы и желаемый тип маршрутизации.
        UInt16 totalLen;                // Общая длина. Занимает 2 байта и указывает общую длину пакета с учетом заголовка и поля данных.
        UInt16 id;                      // Идентификатор пакета. Занимает 2 байта и используется для распознавания пакетов, образовавшихся путем фрагментации исходного пакета.
        UInt16 flagsAndOffset;          // Первые 3 бита - флаги фрагментации. Следующие 13 - смещение поля данных этого пакета от начала общего поля данных исходного пакета, подвергнутого фрагментации.
        Byte ttl;                       // Время жизни пакета.
        Byte protocol;                  // Протокол верхнего уровня. (TCP, UDP или RIP).
        Int16 checksum;                 // Контрольная сумма заголовка. Занимает 2 байта, рассчитывается по всему заголовку.
        UInt32 sourceIP;                // Адрес источника. (4 байта).
        UInt32 destIP;                  // Адрес получателя. (4 байта).

        Byte[] data;                    // Сообщение, содержащееся после заголовка.

        Byte headerLength;              // Длина заголовка.
        UInt16 messageLength;           // Длина сообщения.
        
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
                versionAndHeaderLength = binaryReader.ReadByte();
                serviceType = binaryReader.ReadByte();
                totalLen = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                id = (UInt16)IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                flagsAndOffset = (UInt16)IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                ttl = binaryReader.ReadByte();
                protocol = binaryReader.ReadByte();
                checksum = IPAddress.HostToNetworkOrder(binaryReader.ReadInt16());
                sourceIP = (UInt32)binaryReader.ReadInt32();
                destIP = (UInt32)binaryReader.ReadInt32();

                headerLength = versionAndHeaderLength;
                headerLength <<= 4;
                headerLength >>= 4;
                headerLength *= 4;  // Т.к. поле headerLength содержит в себе количество 32х-битных слов, домножаем на 4, чтобы получить количество байт.

                messageLength = (UInt16)(totalLen - headerLength);

                data = new byte[messageLength];
                Array.Copy(Buffer, headerLength, data, 0, data.Length);
            }
        }

        /// <summary>
        /// Версия протокола.
        /// </summary>
        public String Version
        {
            get
            {
                Byte version = (Byte)(versionAndHeaderLength >> 4);
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
            get { return headerLength; }
        }

        /// <summary>
        /// Тип сервиса. Задает приоритет дейтаграммы и желаемый тип маршрутизации.
        /// </summary>
        public String ServiceType
        {
            get { return String.Format("0x{0:x2} ({0})", serviceType); }
        }

        /// <summary>
        /// Общая длина всего IP пакета.
        /// </summary>
        public UInt16 TotalLen
        {
            get { return totalLen; }
        }

        /// <summary>
        /// Идентификатор пакета. Используется для распознавания пакетов после фрагментации.
        /// </summary>
        public UInt16 Id
        {
            get { return id; }
        }

        /// <summary>
        /// Флаги фрагментации.
        /// </summary>
        public String Flags
        {
            get
            {
                UInt16 flags = (UInt16)(flagsAndOffset >> 13);
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
                UInt16 offset = (UInt16)(flagsAndOffset << 3);
                offset >>= 3;
                return offset;
            }
        }

        /// <summary>
        /// Время жизни пакета.
        /// </summary>
        public Byte TTL
        {
            get { return ttl; }
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
                switch (protocol)
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
            get { return "0x" + checksum.ToString("x"); }
        }

        /// <summary>
        /// Адрес отправителя.
        /// </summary>
        public IPAddress SourceIP
        {
            get { return new IPAddress(sourceIP); }
        }

        /// <summary>
        /// Адрес получателя.
        /// </summary>
        public IPAddress DestinationIP
        {
            get { return new IPAddress(destIP); }
        }

        /// <summary>
        /// Длина сообщения из IP пакета.
        /// </summary>
        public UInt16 MessageLength
        {
            get { return messageLength; }
        }

        /// <summary>
        /// Сообщение из IP пакета.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }
    }
}