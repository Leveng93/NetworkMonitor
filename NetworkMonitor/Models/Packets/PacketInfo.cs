using System;
using System.Collections.Generic;

namespace NetworkMonitor.Models.Packets
{
    class PacketInfo
    {
        #region Properties

        public PacketIP PacketIp { get; private set; }
        public ulong PacketNumber { get; private set; }
        public DateTime ReceiveTime { get; private set; }

        public IGroupedData UpLevelProtocolGroupedData { get; private set; }

        #endregion // Properties

        #region Constructors

        PacketInfo() { }

        public PacketInfo(PacketIP packetIp, ulong packetNumber = default(ulong), DateTime receiveTime = default(DateTime))
        {
            PacketIp = packetIp;
            PacketNumber = packetNumber;
            ReceiveTime = receiveTime;
        }

        #endregion // Constructors
    }
}
