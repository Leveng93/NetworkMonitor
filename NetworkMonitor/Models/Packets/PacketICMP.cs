using System;
using System.IO;
using System.Net;

namespace NetworkMonitor.Models.Packets
{
    class PacketICMP
    {
        #region Fields

        Byte _type;          // Тип сообщения ICMP.
        Byte _code;          // Код ошибки.
        Int16 _checksum;     // Контрольная сумма.

        Byte[] _data;        // Сообщение, содержащееся после заголовка. Зависит от полей type и code.

        #endregion  // Fields

        #region Constructors

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
                _type = binaryReader.ReadByte();
                _code = binaryReader.ReadByte();
                _checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                _data = new byte[Received - 4];
                Array.Copy(Buffer, 4, _data, 0, _data.Length);
            }
        }

        #endregion // Constructors

        #region Properties

        public byte Type
        {
            get { return _type; }
        }

        public byte Code
        {
            get { return _code; }
        }

        public short Checksum
        {
            get { return _checksum; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        #endregion // Properties
    }
}
