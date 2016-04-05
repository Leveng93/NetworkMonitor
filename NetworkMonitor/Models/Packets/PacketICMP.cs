using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NetworkMonitor.Models.Packets
{
    class PacketICMP : IGroupedData<string>
    {
        List<string> _groupedData;
        public IEnumerable<string> GroupedData
        {
            get
            {
                if (_groupedData != null) return _groupedData.AsReadOnly();

                _groupedData = new List<string>();

                _groupedData.Add("Message type: " + Type);
                _groupedData.Add("Error code: " + Code);
                _groupedData.Add("Check sum: " + Checksum);

                return _groupedData.AsReadOnly();
            }
        }

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

        public string Checksum
        {
            get { return "0x" + _checksum.ToString("x"); }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        #endregion // Properties
    }
}
