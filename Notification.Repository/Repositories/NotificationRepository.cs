using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Notification.Model;

namespace Notification.Repository.Repositories
{
    public class NotificationRepository : IRepository<Model.Notification>
    {
        public IQueryable<Model.Notification> Get(Func<Model.Notification, bool> expression)
        {
            using (var dbContex = new NotificationContext(new DbContextOptions<NotificationContext>()))
            {
                return dbContex.Notifications.Where(s => expression(s)).ToArray().AsQueryable();
            }
        }

        public void Greate(IEnumerable<Model.Notification> type)
        {
            using (var dbContext = new NotificationContext(new DbContextOptions<NotificationContext>()))
            {
                dbContext.Notifications.AddRange(type);
                dbContext.SaveChanges();
            }
        }
    }
}
