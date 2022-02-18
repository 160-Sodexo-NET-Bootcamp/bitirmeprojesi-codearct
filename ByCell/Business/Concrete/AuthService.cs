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
    //Yetkilendirme ve kimlik doğrulama işlemleri
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

        //Kullanıcı kayıt işlemi yapılır
        //Kullanıcının girdiği veriler method başlamadan kontrol edilir
        //Başarılı ise method çalışır
        //Başarısız ise kullanıcıya validasyona bağlı olarak hata fırlatılır
        [ValidationAspect(typeof(UserForRegisterValidator))]
        public IResult Register(UserForRegisterDto userForRegisterDto)
        {
            byte[] passwordHash, passwordSalt;
            //Kullanıcıdan gelen şifre haslenir ve saltlanır
            HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);
            //Kullanıcıdan gelen verilerle vertabanı objesi oluşturulur
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

            //Veritabanına userservice üzerinden kayıt atılır
            var result = _userService.CreateUser(user);
            if (!result)
            {
                //Userservice ten hata dönerse kullanıcıya ilgili mesaj dönülür
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            //Kayıt başarılı olursa kullanıcıya ilgili başarı mesajı dönülür
            return new SuccessResult(Messages.UserCreated);
        }

        //Kullanıcı giriş işlemi yapılır
        //Kullanıcının girdiği veriler method başlamadan kontrol edilir
        //Başarılı ise method çalışır
        //Başarısız ise kullanıcıya validasyona bağlı olarak hata fırlatılır
        [ValidationAspect(typeof(UserForLoginValidator))]
        public IDataResult<AccessToken> Login(UserForLoginDto userForLoginDto)
        {
            //Verilen email bilgisiyle veritabanından kullanıcı aranır
            var user = _userService.GetByMail(userForLoginDto.Email);
            //Kullanıcı var fakat bloklıysa ilgili hata kullanıcıya dönülür
            if (user.Blocked==true)
            {
                return new ErrorDataResult<AccessToken>(Messages.UserBlocked);
            }

            //Kullanıcı yoksa ilgili hata kullanıcıya dönülür
            if (user == null)
            {
                return new ErrorDataResult<AccessToken>(Messages.UserNotFound);
            }

            //3 ten fazla yanlış şifre kontrolü
            //Sürekli veritabanı işlemleri yapılmaması için tryCount redis tarafından tutuldu
            //benzersiz bir key oluşturulur
            string cacheKey = "Login_" + userForLoginDto.Email;
            int tryCount = 0;
            //rediste daha önce kayıt varsa ilgili key tarafından devam edilir
            //yoksa tryCount sıfır olacak şeklde yeni veri oluşturulur
            if (!_cacheService.IsAdd(cacheKey))
                _cacheService.Add(cacheKey, tryCount);

            //Redisteki bu key ile oluşturulan tryCount değeri çekilir
            tryCount = _cacheService.Get<int>(cacheKey);
            
            //tryCount 3 ten küçük ve şifre yanlışsa
            if (!HashingHelper.CheckPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt)&&tryCount<3)
            {    
                //Mevcut tryCount değeri silinir
                _cacheService.Remove(cacheKey);
                //tryCount 1 arttırılır
                tryCount++;
                //Yeni değer tekrar aynı key ile redise atılır
                _cacheService.Add(cacheKey, tryCount);
                //Kullanıcıya hata mesajı dönülür
                return new ErrorDataResult<AccessToken>(Messages.PasswordError);
            }

            //3 denemeden sonra
            if(tryCount>=3)
            {
                //Redis kaydı silinir
                _cacheService.Remove(cacheKey);
                //Kullanıcı engellenir
                _userService.BlockUser(userForLoginDto.Email);
                //Kullanıcı bilgilendirme maili veritabanı kuyruğuna alınır
                _mailService.SendBlockedUserMailAsync(userForLoginDto.Email);
                //Kullanıcıya ilgili hata dönülür
                return new ErrorDataResult<AccessToken>(Messages.UserBlocked);
            }

            //Logout işlemi için status sekmesi tutuldu
            if (user.Status == false)
            {
                user.Status = true;
            }

            //Bütün aşamalardan geçen kullanıcı için accesstoken ve refresh token üretilirüretilir
            var accessToken = _tokenHelper.CreateToken(user);
            user.RefreshToken = accessToken.RefreshToken;
            user.RefreshTokenExpiration = accessToken.Expiration.AddMinutes(5);
            //üretilen refresh token ilgili kullanıcı için veritababnına kaydedilir
            _userService.EditByAccessToken(user);

            //Kullanıcıya başarılı mesajı dönülür
            return new SuccessDataResult<AccessToken>(accessToken, Messages.SuccessfulLogin);
        }

        //Mevcut refresh token değeriyle yeni bir accesstoken ve refresh token üretilir
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
