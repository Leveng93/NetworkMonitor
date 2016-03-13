using System;
using System.IO;
using System.Net;
using System.Text;

namespace NetworkMonitor.Packets
{
    [Flags]
    public enum FlagsTCP : UInt16
    {
        FYN = 1,    // Флаг окончания передачи со стороны отправителя.
        SYN = 2,    // Синхронизация чисел последовательности.
        RST = 4,    // Переустановка соединения.
        PSH = 8,    // Флаг форсированной отправки.
        ACK = 16,   // Флаг пакета, содержащего подтверждение получения.
        URG = 32    // Флаг срочности.        
    }

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

        FlagsTCP flags;

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

                UInt16 fl = (UInt16)(dataOffsetAndFlags << 10);
                fl >>= 10;
                flags = (FlagsTCP)fl;                
                
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
                if(flags.HasFlag(FlagsTCP.ACK))
                    return acknowledgmentNumber;
                return 0;
            }
        }

        public Int32 DataOffset
        {
            get { return dataOffsetAndFlags >> 12; }
        }

        public String Flags
        {
            get { return string.Format("{0}", flags); }
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
                if (flags.HasFlag(FlagsTCP.URG))
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

        public bool HasFlag (FlagsTCP flag)
        {
            return flags.HasFlag(flag);
        }
    }
}
