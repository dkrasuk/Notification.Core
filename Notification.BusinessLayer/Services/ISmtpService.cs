using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notification.BusinessLayer.Services
{
    public interface ISmtpService
    {
        Task SendAsync(string emailTo, string body, string chanel);
    }
}
