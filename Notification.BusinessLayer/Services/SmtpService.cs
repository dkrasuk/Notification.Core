using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Notification.BusinessLayer.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly ILogger<SmtpService> _logger;
        private readonly IConfiguration _configuration;

        public SmtpService(ILogger<SmtpService> logger, IConfiguration configuration )
        {
            _logger = logger;
            _configuration = configuration;
        }

        private async Task SendAsync(string emailTo, string body, SmtpClient smtp, string chanel)
        {
            string emailFrom = _configuration["smtpSendFrom"];

            string subject = chanel;

            var mail = new MailMessage();

            mail.From = new MailAddress(emailFrom);

            mail.To.Add(emailTo);
            mail.Subject = subject;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, "text/html"));

            try
            {
                await smtp.SendMailAsync(mail);
                _logger.LogInformation($"Sent email to {emailTo}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error to send email to {emailTo}", e);
            }
        }

        public async Task SendAsync(string[] emailsTo, string body, string chanel)
        {
            using (SmtpClient smtp = GetSmtpClient())
            {
                smtp.UseDefaultCredentials = true;
                foreach (var email in emailsTo)
                {
                    await SendAsync(email, body, smtp, chanel);
                }
            }
        }

        public async Task SendAsync(string emailTo, string body, string chanel)
        {
            await SendAsync(new[] { emailTo }, body, chanel);
        }

        private SmtpClient GetSmtpClient()
        {
            int portNumber = 587;
            int.TryParse(_configuration["smtpPortNumber"], out portNumber);
            string smtpAddress =_configuration["smtpHostAddress"];
            return new SmtpClient(smtpAddress, portNumber);
        }
    }
}
