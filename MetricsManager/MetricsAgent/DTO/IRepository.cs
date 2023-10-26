using System.Collections.Generic;

namespace MetricsAgent.DTO
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();
        void Create(T item);        
        IList<T> GetByTimePeriod(long fromTime, long toTime);
    }
}
