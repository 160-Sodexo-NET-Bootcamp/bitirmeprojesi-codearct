using Business.Abstract;
using Business.Constants;
using Business.DTOs.UserDTOs;
using Business.ValidationRules.FluentValidation.UserValidation;
using Core.Aspects.Autofac.Validation;
using Core.Caching;
using Core.Entities.Concrete;
using Core.Results;
using Core.Security.Hashing;
using Core.Security.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthService:IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICacheService _cacheService;
        private readonly IMailService _mailService;

        public AuthService(IUserService userService, ITokenHelper tokenHelper, ICacheService cacheService, IMailService mailService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _cacheService = cacheService;
            _mailService = mailService;
        }

        [ValidationAspect(typeof(UserForRegisterValidator))]
        public IResult Register(UserForRegisterDto userForRegisterDto)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RefreshToken=string.Empty,
                RefreshTokenExpiration=DateTime.Now,
                Status = true,
                Blocked =false
            };

            var result = _userService.CreateUser(user);
            if (!result)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult(Messages.UserCreated);
        }

        [ValidationAspect(typeof(UserForLoginValidator))]
        public IDataResult<AccessToken> Login(UserForLoginDto userForLoginDto)
        {
            var user = _userService.GetByMail(userForLoginDto.Email);
            if (user.Blocked==true)
            {
                return new ErrorDataResult<AccessToken>(Messages.UserBlocked);
            }

            if (user == null)
            {
                return new ErrorDataResult<AccessToken>(Messages.UserNotFound);
            }

            string cacheKey = "Login_" + userForLoginDto.Email;
            int tryCount = 0;
            if (!_cacheService.IsAdd(cacheKey))
                _cacheService.Add(cacheKey, tryCount);

            tryCount = _cacheService.Get<int>(cacheKey);

            if (!HashingHelper.CheckPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt)&&tryCount<3)
            {               
                _cacheService.Remove(cacheKey);
                tryCount++;
                _cacheService.Add(cacheKey, tryCount);
                return new ErrorDataResult<AccessToken>(Messages.PasswordError);
            }
            if(tryCount>=3)
            {
                _cacheService.Remove(cacheKey);
                _userService.BlockUser(userForLoginDto.Email);
                _mailService.SendBlockedUserMailAsync(userForLoginDto.Email);
                return new ErrorDataResult<AccessToken>(Messages.UserBlocked);
            }

            if (user.Status == false)
            {
                user.Status = true;
            }


            var accessToken = _tokenHelper.CreateToken(user);
            user.RefreshToken = accessToken.RefreshToken;
            user.RefreshTokenExpiration = accessToken.Expiration.AddMinutes(5);
            _userService.EditByAccessToken(user);

            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }

        public IDataResult<AccessToken> RefreshToken(string existingRefreshToken)
        {
            var user = _userService.GetByRefreshToken(existingRefreshToken);
            if (user is null)
            {
                return new ErrorDataResult<AccessToken>(Messages.UnValidRefreshToken);
            }

            var refreshedToken = _tokenHelper.CreateToken(user);
            user.RefreshToken = refreshedToken.RefreshToken;
            user.RefreshTokenExpiration = refreshedToken.Expiration.AddMinutes(5);
            _userService.EditByAccessToken(user);

            return new SuccessDataResult<AccessToken>(refreshedToken);
        }
    }
}
