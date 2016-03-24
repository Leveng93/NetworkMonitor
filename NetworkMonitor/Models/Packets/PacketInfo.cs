using System;

namespace NetworkMonitor.Models.Packets
{
    class PacketInfo
    {
        #region Properties

        public PacketIP _packetIp { get; private set; }
        public ulong _packetNumber { get; private set; }
        public DateTime _receiveTime { get; private set; }

        #endregion // Properties

        #region Constructors

        PacketInfo() { }

        public PacketInfo(PacketIP packetIp, ulong packetNumber = default(ulong), DateTime receiveTime = default(DateTime))
        {
            _packetIp = packetIp;
            _packetNumber = packetNumber;
            _receiveTime = receiveTime;
        }

        #endregion // Constructors
    }
}
