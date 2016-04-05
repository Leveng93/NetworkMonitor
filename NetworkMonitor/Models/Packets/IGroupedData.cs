using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Models.Packets
{
    interface IGroupedData<T>
    {
        IEnumerable<T> GroupedData { get; }
    }

    interface ITestInterface
    {
        IEnumerable<T> GroupData<T>();
    }
}
