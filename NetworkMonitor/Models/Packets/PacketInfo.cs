using System;

namespace NetworkMonitor.Models.Packets
{
    /// <summary>
    /// Информация о принятом сетевом пакете.
    /// </summary>
    class PacketInfo
    {
        #region Properties

        public PacketIP PacketIp { get; private set; }
        public ulong PacketNumber { get; private set; }
        public DateTime ReceiveTime { get; private set; }

        public IGroupedData<string> UpLevelProtocolGroupedData { get; private set; } // Протокол верхнего уровня ISO/OSI

        #endregion // Properties

        #region Constructors

        PacketInfo() { }

        public PacketInfo(PacketIP packetIp, ulong packetNumber = default(ulong), DateTime receiveTime = default(DateTime))
        {
            PacketIp = packetIp;
            PacketNumber = packetNumber;
            ReceiveTime = receiveTime;
            UpLevelProtocolGroupedData = GetUpLevelProtocol(packetIp);
        }

        #endregion // Constructors

        /// <summary>
        /// Получает протокол верхнего уровня на основании свойства PacketIP.Protocol.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        IGroupedData<string> GetUpLevelProtocol(PacketIP packet)
        {
            switch (packet.Protocol)
            {
                case "TCP": return new PacketTCP(packet);
                case "UDP": return new PacketUDP(packet);
                case "ICMP": return new PacketICMP(packet);
                case "IGMP": break; // Реализовать
            }
            return null;
        }
    }
}
