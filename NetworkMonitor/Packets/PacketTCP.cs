using System;
using System.IO;
using System.Net;

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


        public ushort SourcePort
        {
            get
            {
                return sourcePort;
            }
        }

        public ushort DestinationPort
        {
            get
            {
                return destinationPort;
            }
        }
    }
}
