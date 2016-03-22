using System;
using System.IO;
using System.Net;

namespace NetworkMonitor.Models.Packets
{
    class PacketICMP
    {
        Byte type;          // Тип сообщения ICMP.
        Byte code;          // Код ошибки.
        Int16 checksum;     // Контрольная сумма.

        Byte[] data;        // Сообщение, содержащееся после заголовка. Зависит от полей type и code.
        
        /// <summary>
        /// Инициализирует новый экземпляр класса PacketICMP.
        /// </summary>
        /// <param name="PackIP">Экземпляр класса PacketIP</param>
        public PacketICMP(PacketIP PackIP) : this(PackIP.Data, PackIP.MessageLength) { }

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketICMP.
        /// </summary>
        /// <param name="Buffer">Массив байт для парсинга</param>
        /// <param name="Received">Количество байт в массиве</param>
        public PacketICMP(Byte[] Buffer, Int32 Received)
        {
            using (MemoryStream memoryStream = new MemoryStream(Buffer, 0, Received))
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                type = binaryReader.ReadByte();
                code = binaryReader.ReadByte();
                checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                data = new byte[Received - 4];
                Array.Copy(Buffer, 4, data, 0, data.Length);
            }
        }

        public byte Type
        {
            get { return type; }
        }

        public byte Code
        {
            get { return code; }
        }

        public short Checksum
        {
            get { return checksum; }
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}
