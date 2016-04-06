using System.Collections.Generic;

namespace NetworkMonitor.Models.Packets
{
    interface IGroupedData
    {
        IEnumerable<string> GetGroupedData();
    }
}