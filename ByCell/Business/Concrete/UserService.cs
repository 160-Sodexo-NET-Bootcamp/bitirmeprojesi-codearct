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

        public bool CreateUser(User user)
        {
            var newUser = GetByMail(user.Email);
            if (newUser != null && newUser.Blocked==true)
            {
                return false;
            }
            _uow.Users.Add(user);
            _uow.Commit();
            return true;
        }

        public User GetByMail(string email)
        {
            return _uow.Users.Get(u => u.Email == email);
        }

        public User GetByRefreshToken(string existingRefreshToken)
        {
            return _uow.Users.Get(u => u.RefreshToken == existingRefreshToken 
                                               && u.RefreshTokenExpiration > DateTime.Now);
        }

        public void EditByAccessToken(User user)
        {
            _uow.Users.Update(user);
            _uow.Commit();
        }

        public void BlockUser(string emailAdress)
        {
            var user = _uow.Users.Get(u => u.Email == emailAdress);
            user.Blocked = true;
            _uow.Users.Update(user);
            _uow.Commit();
        }
    }
}
