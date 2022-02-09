using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        //Kullanıcı kayıt,giriş ve doğrulama mesajları
        public static string UserCreated => "Kayıt oluşturuldu.";
        public static string UserNotFound => "Kullanıcı bulunamadı.";
        public static string PasswordError => "Giriş bilgilerini kontrol edin!";
        public static string SuccessfulLogin => "Hoşgeldiniz!";
        public static string UserAlreadyExists => "Kullanıcı mevcut!";
        public static string UnValidRefreshToken => "Tekrar giriş yapınız.";
    }
}
