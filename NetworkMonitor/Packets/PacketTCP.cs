using System;
using System.IO;
using System.Net;
using System.Text;

namespace NetworkMonitor.Packets
{
    class PacketTCP
    {
        UInt16 sourcePort;              // Порт источника. 2 байта.
        UInt16 destinationPort;         // Порт назначения. 2 байта.
        UInt32 sequenceNumber;          // Последовательный номер. 4 байта. Указывает номер байта, который определяет смещение сегмента относительно потока отправляемых данных.
        UInt32 acknowledgmentNumber;    // Подтвержденный номер. 4 байта. Содержит максимальный номер байта в полученном сегменте, увеличенный на единицу. (Квитанция).
        UInt16 dataOffsetAndFlags;      // Количество 32-битных слов в TCP заголовке. 4 бита. Далее 6 бит резеврное поле (нули). Далее 6 контрольных бит.
        UInt16 window;                  // Количество октетов данных, начиная с октета, чей номер указан в поле подтверждения. 2 байта.
        Int16 checksum;                 // Контрольная сумма. 2 байта.
        UInt16 urgentPointer;           // Срочный указатель: сообщает номер очереди для октета, следующего за срочными данными. 2 байта.
        UInt32 optionsAndPading;        // Опции - 3 байта. Заполнитель - переменная длина, чтобы довести размер заголовка до целого числа 32х-битных слов.

        Byte headerLength;              // Длина заголовка.
        UInt16 messageLength;           // Длина сообщения.

        byte[] data;

        public PacketTCP(PacketIP PackIP) : this(PackIP.Data, PackIP.MessageLength) { }

        public PacketTCP(Byte[] Buffer, Int32 Received)
        {
            using (MemoryStream memoryStream = new MemoryStream(Buffer, 0, Received))
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                sourcePort = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                destinationPort = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                sequenceNumber = (UInt32)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                acknowledgmentNumber = (UInt32)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                dataOffsetAndFlags = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                window = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                checksum = (Int16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                urgentPointer = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                optionsAndPading = (UInt32)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

                headerLength = (Byte)(dataOffsetAndFlags >> 12);
                headerLength *= 4;

                messageLength = (UInt16)(Received - headerLength);
                data = new byte[messageLength];
                Array.Copy(Buffer, headerLength, data, 0, data.Length);
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
        /// Последовательный номер.
        /// </summary>
        public UInt32 SequenceNumber
        {
            get { return sequenceNumber; }
        }

        public UInt32 AcknowledgmentNumber
        {
            get
            {
                if ((dataOffsetAndFlags & 0x10) != 0) 
                    return acknowledgmentNumber;
                return 0;
            }
        }

        public Int32 DataOffset
        {
            get
            {
                return dataOffsetAndFlags >> 12;
            }
        }

        public String Flags
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if ((dataOffsetAndFlags & 0x20) != 0) sb.Append("URG "); // Флаг срочности.
                if ((dataOffsetAndFlags & 0x10) != 0) sb.Append("ACK "); // Флаг пакета, содержащего подтверждение получения.
                if ((dataOffsetAndFlags & 0x8) != 0) sb.Append("PSH ");  // Флаг форсированной отправки.
                if ((dataOffsetAndFlags & 0x4) != 0) sb.Append("RST ");  // Переустановка соединения.
                if ((dataOffsetAndFlags & 0x2) != 0) sb.Append("SYN ");  // Синхронизация чисел последовательности.
                if ((dataOffsetAndFlags & 0x1) != 0) sb.Append("FYN ");  // Флаг окончания передачи со стороны отправителя.

                return sb.ToString();
            }
        }

        public UInt16 Window
        {
            get { return window; }
        }

        public Int16 Checksum
        {
            get { return checksum; }
        }

        public UInt16 UrgentPointer
        {
            get
            {
                if ((dataOffsetAndFlags & 0x20) != 0)
                    return urgentPointer;
                return 0;
            }
        }

        public Byte HeaderLength
        {
            get { return headerLength; }
        }

        public UInt16 MessageLength
        {
            get { return messageLength; }
        }

        public Byte[] Data
        {
            get { return data; }
        }
    }
}
