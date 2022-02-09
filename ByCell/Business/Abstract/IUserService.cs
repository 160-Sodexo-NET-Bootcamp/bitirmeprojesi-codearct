using Core.Entities.Concrete;
using Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        bool CreateUser(User user);
        void EditByAccessToken(User user);
        User GetByMail(string email);
        User GetByRefreshToken(string existingRefreshToken);
    }
}
