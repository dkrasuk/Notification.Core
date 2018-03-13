using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Repository.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Model.Notification>
    {
        public void Configure(EntityTypeBuilder<Model.Notification> builder)
        {

        }
    }
}
