using System;
using System.Collections.Generic;
using System.Linq;

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

        public string[] PacketIPGroupedData { get; private set; }
        public string[] UpLevelProtocolGroupedData { get; private set; } // Протокол верхнего уровня ISO/OSI

        #endregion // Properties

        #region Constructors

        PacketInfo() { }

        public PacketInfo(PacketIP packetIp, ulong packetNumber = default(ulong), DateTime receiveTime = default(DateTime))
        {
            PacketIp = packetIp;
            PacketNumber = packetNumber;
            ReceiveTime = receiveTime;
            var upLevelProtocol = GetUpLevelProtocolData(packetIp);
            if (upLevelProtocol != null)
                UpLevelProtocolGroupedData = upLevelProtocol.ToArray();
            PacketIPGroupedData = PacketIp.GetGroupedData().ToArray();
        }

        #endregion // Constructors

        /// <summary>
        /// Получает протокол верхнего уровня на основании свойства PacketIP.Protocol.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        IEnumerable<string> GetUpLevelProtocolData(PacketIP packet)
        {
            switch (packet.Protocol)
            {
                case "TCP": return new PacketTCP(packet).GetGroupedData();
                case "UDP": return new PacketUDP(packet).GetGroupedData();
                case "ICMP": return new PacketICMP(packet).GetGroupedData();
                case "IGMP": break; // Реализовать
            }
            return null;
        }
    }
}
