using Business.Abstract;
using Business.DTOs.Mail;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class MailService : IMailService
    {
        public IConfiguration Configuration { get; }
        private readonly SmtpConfigDto _smtpConfigDto;
        private readonly IUserService _userService;

        public MailService(IConfiguration configuration, IUserService userService)
        {
            Configuration = configuration;
            _smtpConfigDto = Configuration.GetSection("SmtpConfig").Get<SmtpConfigDto>();
            _userService = userService;
        }

        public async Task SendRegisteredUserMailAsync(string emailAdress)
        {
            using var client = CreateSmtpClient();
            var userInfo = _userService.GetByMail(emailAdress);

            MailMessageDto mailMessageDto = new MailMessageDto
            {
                Body = $"Sayın {userInfo.FirstName} {userInfo.LastName}," +
                       $"\nByCell'e Hoşgeldiniz" +
                       $"\nSaygılarımızla",
                To = userInfo.Email,
                Subject = "Hoşgeldiniz",
                From = _smtpConfigDto.User
            };
            MailMessage mailMessage = GetMailMessage(mailMessageDto);
            mailMessage.IsBodyHtml = true;
            await client.SendMailAsync(mailMessage);
        }

        private SmtpClient CreateSmtpClient()
        {
            SmtpClient smtp = new SmtpClient(_smtpConfigDto.Host, _smtpConfigDto.Port);
            smtp.Credentials = new NetworkCredential(_smtpConfigDto.User, _smtpConfigDto.Password);
            smtp.EnableSsl = _smtpConfigDto.UseSsl;
            return smtp;
        }

        public async Task SendMailAsync(MailMessageDto mailMessageDto)
        {
            MailMessage mailMessage = GetMailMessage(mailMessageDto);
            mailMessage.From = new MailAddress(_smtpConfigDto.User);

            using var client = CreateSmtpClient();
            await client.SendMailAsync(mailMessage);
        }

        public MailMessage GetMailMessage(MailMessageDto mailMessageDto)
        {
            var mailMessage = new MailMessage
            {
                Subject = mailMessageDto.Subject,
                Body = mailMessageDto.Body,
                From = new MailAddress(mailMessageDto.From)
            };
            mailMessage.To.Add(mailMessageDto.To);
            return mailMessage;
        }
    }
}
