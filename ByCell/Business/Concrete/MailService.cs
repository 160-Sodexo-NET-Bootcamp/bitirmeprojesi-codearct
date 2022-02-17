using Business.Abstract;
using Business.DTOs.Mail;
using Business.Enums;
using Core.Caching;
using Core.Entities.Concrete;
using DataAccess.Abstract;
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
        private readonly ICacheService _cacheService;
        private readonly SmtpConfigDto _smtpConfigDto;
        private readonly IUnitOfWork _uow;

        public MailService(IConfiguration configuration, IUnitOfWork uow, ICacheService cacheService)
        {
            Configuration = configuration;
            _smtpConfigDto = Configuration.GetSection("SmtpConfig").Get<SmtpConfigDto>();
            _uow = uow;
            _cacheService = cacheService;
        }

        public async Task SendRegisteredUserMailAsync(string emailAdress)
        {
            var user = _uow.Users.Get(u => u.Email == emailAdress);

            MailMessageDto mailMessageDto = new MailMessageDto
            {
                Body = $"Sayın {user.FirstName} {user.LastName}," +
                       $"\nByCell'e Hoşgeldiniz" +
                       $"\nSaygılarımızla",
                To = user.Email,
                Subject = "Hoşgeldiniz",
                From = _smtpConfigDto.User
            };
            await CreateMailAsync(mailMessageDto);
        }

        public async Task SendBlockedUserMailAsync(string emailAdress)
        {
            var user = _uow.Users.Get(u => u.Email == emailAdress);

            MailMessageDto mailMessageDto = new MailMessageDto
            {
                Body = $"Sayın {user.FirstName} {user.LastName}," +
                       $"\nHesabınız engellenmiştir.Lütfen yeni kayıt oluşturunuz." +
                       $"\nSaygılarımızla",
                To = user.Email,
                Subject = "Hesap Engellendi!",
                From = _smtpConfigDto.User
            };
            await CreateMailAsync(mailMessageDto);
        }

        private SmtpClient CreateSmtpClient()
        {
            SmtpClient smtp = new SmtpClient(_smtpConfigDto.Host, _smtpConfigDto.Port);
            smtp.Credentials = new NetworkCredential(_smtpConfigDto.User, _smtpConfigDto.Password);
            smtp.EnableSsl = _smtpConfigDto.UseSsl;
            return smtp;
        }

        public async Task SendMailAsync()
        {
            var sentMails = _uow.SentMails.GetAll(m => m.Status == MailStatus.Beklemede.ToString())
                                          .OrderBy(m=>m.Id)
                                          .Take(10)
                                          .ToList();
            int tryCount = 0;
            foreach (var sentMail in sentMails)
            {
                string cacheKey =$"{sentMail.To}_{sentMail.Id}";
                var result = _cacheService.IsAdd(cacheKey);
                if (!result)
                    _cacheService.Add(cacheKey, tryCount);

                try
                {
                    MailMessage mailMessage = new MailMessage
                    {
                        Subject = sentMail.Subject,
                        Body = sentMail.Body,
                        From = new MailAddress(sentMail.From)
                    };
                    mailMessage.To.Add(sentMail.To);
                    mailMessage.From = new MailAddress(_smtpConfigDto.User);

                    using var client = CreateSmtpClient();
                
                    await client.SendMailAsync(mailMessage);
                    ChangeMailStatus(sentMail.Id, true);
                    _cacheService.Remove(cacheKey);
                }
                catch (Exception)
                {
                    tryCount =_cacheService.Get<int>(cacheKey);
                    if (tryCount==5)
                    {
                        ChangeMailStatus(sentMail.Id, false);
                        _cacheService.Remove(cacheKey);
                        throw;
                    }
                    tryCount++;
                    _cacheService.Remove(cacheKey);
                    _cacheService.Add(cacheKey, tryCount);
                }                                
            }
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

        public async Task CreateMailAsync(MailMessageDto mailMessageDto)
        {
            var sentMail = new SentMail
            {
                Subject = mailMessageDto.Subject,
                Body = mailMessageDto.Body,
                From = mailMessageDto.From,
                To = mailMessageDto.To,
                Status = MailStatus.Beklemede.ToString()
            };
            
            _uow.SentMails.Add(sentMail);
            await _uow.CommitAsync();
        }

        public void ChangeMailStatus(int id, bool status)
        {
            var sentMail = _uow.SentMails.Get(m => m.Id == id);
            sentMail.Status = status == false
                            ? MailStatus.Başarısız.ToString()
                            : MailStatus.Gönderildi.ToString();
            _uow.SentMails.Update(sentMail);
            _uow.Commit();
        }
    }
}
