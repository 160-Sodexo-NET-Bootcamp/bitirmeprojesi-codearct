using Business.DTOs.Mail;
using Core.Entities.Concrete;
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
        Task CreateMailAsync(MailMessageDto mailMessageDto);
        void ChangeMailStatus(int id, bool status);
        Task SendMailAsync();
        Task SendRegisteredUserMailAsync(string emailAdress);
        Task SendBlockedUserMailAsync(string emailAdress);
    }
}
