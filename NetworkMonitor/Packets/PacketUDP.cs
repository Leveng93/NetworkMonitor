using System;
using System.IO;
using System.Net;

namespace NetworkMonitor.Packets
{
    /// <summary>
    /// UDP пакет, содержащий в себе заголовок и данные.
    /// </summary>
    class PacketUDP
    {
        UInt16 sourcePort;              // Порт источника. 2 байта.
        UInt16 destinationPort;         // Порт назначения. 2 байта.
        UInt16 totalLength;             // Общая длина UDP пакета. 2 байта.
        Int16 checksum;                 // Контрольная сумма. 2 байта.

        byte[] data;    // Сообщение, содержащееся после заголовка.

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketUDP.
        /// </summary>
        /// <param name="PackIP">Экземпляр класса PacketIP</param>
        public PacketUDP(PacketIP PackIP) : this(PackIP.Data, PackIP.MessageLength) { }

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketUDP.
        /// </summary>
        /// <param name="Buffer">Массив байт для парсинга</param>
        /// <param name="Received">Количество байт в массиве</param>
        public PacketUDP(Byte[] Buffer, Int32 Received)
        {
            using (MemoryStream memoryStream = new MemoryStream(Buffer, 0, Received))
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                sourcePort = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                destinationPort = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                totalLength = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                checksum = (Int16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                data = new byte[totalLength - 8];
                Array.Copy(Buffer, 8, data, 0, data.Length);
            }
        }

        /// <summary>
        /// Порт источника.
        /// </summary>
        public UInt16 SourcePort
        {
            get { return sourcePort; }
        }

        /// <summary>
        /// Порт получателя.
        /// </summary>
        public UInt16 DestinationPort
        {
            get { return destinationPort; }
        }

        /// <summary>
        /// Длина пакета в байтах.
        /// </summary>
        public UInt16 TotalLength
        {
            get { return totalLength; }
        }

        /// <summary>
        /// Контрольная сумма.
        /// </summary>
        public String Checksum
        {
            get { return "0x" + checksum.ToString("x"); }
        }

        /// <summary>
        /// Данные сообщения.
        /// </summary>
        public Byte[] Data
        {
            get { return data; }
        }
    }
}
