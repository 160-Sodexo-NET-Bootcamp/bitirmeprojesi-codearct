using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Results;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        //Kullanıcı eklenir
        public bool CreateUser(User user)
        {
            //Verilen mail ile arama yapılır
            var newUser = GetByMail(user.Email);
            //eğer kullanıcı varsa veya bloklanmışsa false döner
            if (newUser != null && newUser.Blocked==true)
            {
                return false;
            }
            //İş kuralından geçerse kullanıcı veritabanına kaydedilir
            _uow.Users.Add(user);
            _uow.Commit();
            return true;
        }

        //Mail ile kullanıcı aranır
        public User GetByMail(string email)
        {
            return _uow.Users.Get(u => u.Email == email);
        }

        //Refresh token ile kullancı veritabanında aranır
        public User GetByRefreshToken(string existingRefreshToken)
        {
            return _uow.Users.Get(u => u.RefreshToken == existingRefreshToken 
                                               && u.RefreshTokenExpiration > DateTime.Now);
        }

        //Refresh tokenı değişen kullanıcı güncellenir
        public void EditByAccessToken(User user)
        {
            _uow.Users.Update(user);
            _uow.Commit();
        }

        //Verilen mail adresine göre kullanıcı engellenir
        public void BlockUser(string emailAdress)
        {
            var user = _uow.Users.Get(u => u.Email == emailAdress);
            user.Blocked = true;
            _uow.Users.Update(user);
            _uow.Commit();
        }
    }
}
