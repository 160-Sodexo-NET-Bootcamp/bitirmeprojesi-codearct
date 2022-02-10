using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Services.FireAndForgetJobs
{
    public class SendRegisteredUserMail
    {
        private readonly IMailService _mailService;
        public SendRegisteredUserMail(IMailService mailService)
        {
            _mailService = mailService;
        }
        public async Task Process(string emailAdress)
        {

            await _mailService.SendRegisteredUserMailAsync(emailAdress);
            
        }

    }
}
