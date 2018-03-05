using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notification.Repository;
using Notification.Model;
using Microsoft.Extensions.Logging;
using Notification.BusinessLayer.Services.Intarfaces;

namespace NotificationApi.Core.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly NotificationContext _context;
        private readonly ILogger<NotificationController> _logger;
        private readonly ISmtpService _smtpService;

        public NotificationController(NotificationContext context, ILogger<NotificationController> logger, ISmtpService smtpService)
        {
            _context = context;
            _logger = logger;
            _smtpService = smtpService;
        }

        [HttpPost]
        [Route("sendmail")]
        public async Task<IActionResult> SendMail([FromBody]Notification.Model.Notification notification)
        {
            if (notification == null)
                return BadRequest();

            notification.Id = Guid.NewGuid().ToString();

            await Task.Run(() =>
            {
                _context.Notifications.AddAsync(notification);
                _context.SaveChanges();
            });

            await _smtpService.SendAsync(notification.Receiver, notification.Body, notification.Title);            
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("This is OK");
            _logger.LogError("This is Bad!");
            _logger.LogDebug("This is Debug!");
            return Ok(_context.Notifications.ToList().Take(10));

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == null)
                return BadRequest();

            var notify = new Notification.Model.Notification();
            await Task.Run(() =>
            {
                notify = _context.Notifications.FirstOrDefault(c => c.Id == id);
            });

            return Ok(notify);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Notification.Model.Notification notify)
        {
            if (notify == null)
                return BadRequest();

            notify.Id = Guid.NewGuid().ToString();
            await Task.Run(() =>
            {
                _context.Notifications.AddAsync(notify);
                _context.SaveChanges();
            });
            return Ok(notify.Id);
        }


        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody]Notification.Model.Notification notify)
        {
            if (notify == null)
                return BadRequest();

            var notification = new Notification.Model.Notification();
            await Task.Run(() =>
            {
                notification = _context.Notifications.Find(notify.Id);
                _context.Entry(notification).CurrentValues.SetValues(notify);
                _context.SaveChanges();
            });
            return Ok($"Update record {notify.Id} done!");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await Task.Run(() =>
            {
                var notify = _context.Notifications.Where(c => c.Id == id).FirstOrDefault();
                _context.Notifications.Remove(notify);
                _context.SaveChanges();
            });

            return Ok();
        }

    }
}
