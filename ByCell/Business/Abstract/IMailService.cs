using Business.DTOs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IMailService
    {
        Task SendMailAsync(MailMessageDto mailMessageDto);
        Task SendRegisteredUserMailAsync(string emailAdress);
        MailMessage GetMailMessage(MailMessageDto mailMessageDto);
    }
}
