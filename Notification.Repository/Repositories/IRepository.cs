using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notification.Repository.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Get(Func<T, bool> expression);
        void Greate(IEnumerable<T> type);
    }
}
