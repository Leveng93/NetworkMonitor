using System;
using System.IO;
using System.Net;

namespace NetworkMonitor.Models.Packets
{
    /// <summary>
    /// Флаги TCP пакета.
    /// </summary>
    [Flags]
    public enum FlagsTCP : UInt16
    {
        FYN = 1,    // Флаг окончания передачи со стороны отправителя. Нулевой бит.
        SYN = 2,    // Синхронизация чисел последовательности.
        RST = 4,    // Переустановка соединения.
        PSH = 8,    // Флаг форсированной отправки.
        ACK = 16,   // Флаг пакета, содержащего подтверждение получения.
        URG = 32    // Флаг срочности. Пятый бит.      
    }

    /// <summary>
    /// TCP пакет, содержащий в себе заголовок и данные.
    /// </summary>
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

        FlagsTCP flags;                 // Флаги TCP пакета.

        byte[] data;                    // Сообщение, содержащееся после заголовка.

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketTCP.
        /// </summary>
        /// <param name="PackIP">Экземпляр класса PacketIP</param>
        public PacketTCP(PacketIP PackIP) : this(PackIP.Data, PackIP.MessageLength) { }

        /// <summary>
        /// Инициализирует новый экземпляр класса PacketTCP.
        /// </summary>
        /// <param name="Buffer">Массив байт для парсинга</param>
        /// <param name="Received">Количество байт в массиве</param>
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
                checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                urgentPointer = (UInt16)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                optionsAndPading = (UInt32)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());

                headerLength = (Byte)(dataOffsetAndFlags >> 12); // Первые 4 байта в переменной - количество 32х-битных слов.
                headerLength *= 4;  // Переводим в байты.

                messageLength = (UInt16)(Received - headerLength);

                // Записываем в отдельную переменную флаги TCP пакета.
                UInt16 fl = (UInt16)(dataOffsetAndFlags & 0x3F);    // Маска 111111 в двоичной.
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

        /// <summary>
        /// Поле номера кадра подтвержденного получения.
        /// </summary>
        public UInt32 AcknowledgmentNumber
        {
            get
            {
                if(flags.HasFlag(FlagsTCP.ACK))
                    return acknowledgmentNumber;
                return 0;
            }
        }

        /// <summary>
        /// Поле величины смещения данных. Оно содержит количество 32-битных слов заголовка TCP-пакета.
        /// </summary>
        public Int32 DataOffset
        {
            get { return dataOffsetAndFlags >> 12; }
        }

        public String Flags
        {
            get { return string.Format("{0}", flags); }
        }

        /// <summary>
        /// Окно. Содержит количество байт данных, которое отправитель данного сегмента может принять, отсчитанное от номера байта, указанного в поле Acknowledgment Number.
        /// </summary>
        public UInt16 Window
        {
            get { return window; }
        }

        /// <summary>
        /// Контрольная сумма.
        /// </summary>
        public String Checksum
        {
            get { return "0x" + checksum.ToString("x"); }
        }

        /// <summary>
        /// Поле указателя срочных данных. Это поле содержит значение счетчика пакетов, начиная с которого следуют пакеты повышенной срочности.
        /// </summary>
        public UInt16 UrgentPointer
        {
            get
            {
                if (flags.HasFlag(FlagsTCP.URG))
                    return urgentPointer;
                return 0;
            }
        }

        /// <summary>
        /// Длина заголовка TCP пакета в байтах.
        /// </summary>
        public Byte HeaderLength
        {
            get { return headerLength; }
        }

        /// <summary>
        /// Длина сообщения в байтах.
        /// </summary>
        public UInt16 MessageLength
        {
            get { return messageLength; }
        }

        /// <summary>
        /// Данные сообщения.
        /// </summary>
        public Byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Метод, показывающий, содержится ли в данном пакете переданный битовый флаг.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool HasFlag (FlagsTCP flag)
        {
            return flags.HasFlag(flag);
        }
    }
}
