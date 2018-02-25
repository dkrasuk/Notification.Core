using Microsoft.EntityFrameworkCore;
using System;

namespace Notification.Repository
{
    public class NotificationContext : DbContext
    {

        public NotificationContext(DbContextOptions<NotificationContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public virtual DbSet<Notification.Model.Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("dev");
            base.OnModelCreating(builder);
        }

    }
}
