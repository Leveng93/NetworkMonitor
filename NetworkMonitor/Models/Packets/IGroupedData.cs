using System.Collections.Generic;

namespace NetworkMonitor.Models.Packets
{
    /// <summary>
    /// Данные, сгруппированные в IEnumerable<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IGroupedData<T>
    {
        /// <summary>
        /// Возвращает данные, сгруппированные в Enumerable<T>
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetGroupedData();
    }
}