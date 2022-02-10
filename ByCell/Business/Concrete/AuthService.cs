using Business.Abstract;
using Business.Constants;
using Business.DTOs.UserDTOs;
using Business.ValidationRules.FluentValidation.UserValidation;
using Core.Aspects.Autofac.Validation;
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
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthService(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
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

            //Kullanıcıya maille hoşegeldin mesajı atılacak
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

            if (user == null)
            {
                return new ErrorDataResult<AccessToken>(Messages.UserNotFound);
            }

            //Kullanıcı yanlış parola girdiğinde redise counter atılacak
            //redisten bu get edilip kontrol edilip 3 kez yanlış girildiğinde blocked true ya çekilip mail atılacak
            if (!HashingHelper.CheckPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {           
                return new ErrorDataResult<AccessToken>(Messages.PasswordError);
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
