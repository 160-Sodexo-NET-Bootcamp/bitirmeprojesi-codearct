using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundServicesAPI.BackgroundJobs.Services.RecurringJobs
{
    //Hangfire için mail gönderim servisi
    public class SendUserMail
    {
        private readonly IMailService _mailService;
        public SendUserMail(IMailService mailService)
        {
            _mailService = mailService;
        }
        public async Task Process()
        {
            await _mailService.SendMailAsync();
        }

    }
}
