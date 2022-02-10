using Business.DTOs.UserDTOs;
using Core.Entities.Concrete;
using Core.Results;
using Core.Security.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IResult Register(UserForRegisterDto userForRegisterDto );
        IDataResult<AccessToken> Login(UserForLoginDto userForLoginDto);
        IDataResult<AccessToken> RefreshToken(string existingRefreshToken);
    }
}
