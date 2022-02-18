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
            //Configuration ile appsettings.json dan Smtp configurasyon bilgileri objemiz olan SmtpConfigDto a aktarılır
            _smtpConfigDto = Configuration.GetSection("SmtpConfig").Get<SmtpConfigDto>();
            _uow = uow;
            _cacheService = cacheService;
        }

        //Kullanıcı "Hoşgeldiniz" maili oluşturulur ve veritabanında kuyruğa atılır
        public async Task SendRegisteredUserMailAsync(string emailAdress)
        {
            //Veritabanından yeni oluşturulan user bilgileri çekilir
            var user = _uow.Users.Get(u => u.Email == emailAdress);

            MailMessageDto mailMessageDto = new MailMessageDto
            {
                //Kullanıcının adı ve soyadına göre mesaj body si doldurulur
                Body = $"Sayın {user.FirstName} {user.LastName}," +
                       $"\nByCell'e Hoşgeldiniz" +
                       $"\nSaygılarımızla",
                To = user.Email,
                Subject = "Hoşgeldiniz",
                //smtpconfig bilgileriyle hangi mailden mesajın gönderileceği alınır
                From = _smtpConfigDto.User
            };
            await CreateMailAsync(mailMessageDto);
        }

        //Kullanıcıya engellendiğine dair bilgilendirme maili oluşturulur ve veritabanına kuyruğuna atılır
        public async Task SendBlockedUserMailAsync(string emailAdress)
        {
            //Veritabanından user bilgileri çekilir
            var user = _uow.Users.Get(u => u.Email == emailAdress);

            MailMessageDto mailMessageDto = new MailMessageDto
            {
                //Kullanıcının adı ve soyadına göre mesaj body si doldurulur
                Body = $"Sayın {user.FirstName} {user.LastName}," +
                       $"\nHesabınız engellenmiştir.Lütfen yeni kayıt oluşturunuz." +
                       $"\nSaygılarımızla",
                To = user.Email,
                Subject = "Hesap Engellendi!",
                //smtpconfig bilgileriyle hangi mailden mesajın gönderileceği alınır
                From = _smtpConfigDto.User
            };
            await CreateMailAsync(mailMessageDto);
        }

        //Smtp servisi oluşturulur ilgili configurasyonlara bağlı olarak
        private SmtpClient CreateSmtpClient()
        {
            SmtpClient smtp = new SmtpClient(_smtpConfigDto.Host, _smtpConfigDto.Port);
            smtp.Credentials = new NetworkCredential(_smtpConfigDto.User, _smtpConfigDto.Password);
            smtp.EnableSsl = _smtpConfigDto.UseSsl;
            return smtp;
        }

        //Oluşturulan mailleri gönderen mail servisi
        public async Task SendMailAsync()
        {
            //Veritabanında statusü "Bekelemede" olan ilk 0 mail alınır
            var sentMails = _uow.SentMails.GetAll(m => m.Status == MailStatus.Beklemede.ToString())
                                          .OrderBy(m=>m.Id)
                                          .Take(10)
                                          .ToList();
            //Mail gönderim deneme sayısı tryCount olarak tutulur
            int tryCount = 0;
            //Döngüyle her mail gönderilir
            foreach (var sentMail in sentMails)
            {
                //Benzersiz bir key oluşturulur
                string cacheKey =$"{sentMail.To}_{sentMail.Id}";
                //Bu key e ait veri rediste var mı kontrol edilir
                var result = _cacheService.IsAdd(cacheKey);
                //Yoksa yeni kayıt oluşturulur tryCount sıfır olacak şekilde
                //Varsa devam edilir
                if (!result)
                    _cacheService.Add(cacheKey, tryCount);

                //try-count yapısıyla gönderim başarılı mı başarısız mı bakılır
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

                    //Gönderim try içinde kalırsa(yani başarılı) ilgili mailin statusu "Gönderildi" olarak güncellenir
                    ChangeMailStatus(sentMail.Id, true);
                    //Redis den tryCount silinir
                    _cacheService.Remove(cacheKey);
                }
                //Mail gönderimi başarısızsa(catch e düşerse)
                catch (Exception)
                {
                    //Redisden tryCount değeri çekilir
                    tryCount =_cacheService.Get<int>(cacheKey);
                    //Eğer bu değer 5 ise
                    //Mailin statusü "Başarısız" olarak güncellenir;
                    //Redisten trycount verisi silinir ve bir sonraki maile geçilir
                    if (tryCount==5)
                    {
                        ChangeMailStatus(sentMail.Id, false);
                        _cacheService.Remove(cacheKey);
                        continue;
                    }
                    //tryCount 5 den küçükse her başarısız denemede değeri 1 arttırılır
                    tryCount++;
                    //mevcut değer redisten silinir
                    _cacheService.Remove(cacheKey);
                    //Yeni değer redise gönderilir
                    _cacheService.Add(cacheKey, tryCount);
                    continue;
                }                                
            }
        }                           
        
        //Verilen Dto bilgilerine göre SentMail veritabanı objesi oluşturup veritabanına kayıt atan metot
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

        //Verilen Id ve status değerine göre mail statusünü değiştiren metot
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
