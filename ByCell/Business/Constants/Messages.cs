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
        public static string UserCreated => "Kayıt oluşturuldu";
        public static string UserNotFound => "Kullanıcı bulunamadı";
        public static string PasswordError => "Giriş bilgilerini kontrol edin!";
        public static string SuccessfulLogin => "Hoşgeldiniz";
        public static string UserAlreadyExists => "Kullanıcı mevcut!";
        public static string UnValidRefreshToken => "Tekrar giriş yapınız";

        //Category mesajları
        public static string ExistingCategory => "Kategori mevcut";
        public static string CategoryAdded => "Kategori eklendi";
        public static string CategoryNotFound => "Kategori bulunamadı!";
        public static string CategoryAlreadyNotExist => "Kategori zaten mevcut değil!";
        public static string CategoryRemoved => "Kategori silindi!";
        public static string CategoryUpdated => "Kategori güncellendi";

        //Color mesajları
        public static string ExistingColor => "Renk mevcut";
        public static string ColorAdded => "Renk eklendi";
        public static string ColorNotFound => "Renk bulunamadı!";
        public static string ColorAlreadyNotExist => "Renk zaten mevcut değil!";
        public static string ColorRemoved => "Renk silindi!";
        public static string ColorUpdated => "Renk güncellendi";

        //OfferConfirm mesajları
        public static string ExistingOfferConfirm => "Teklif onayı mevcut";
        public static string OfferConfirmAdded => "Teklif onayı eklendi";
        public static string OfferConfirmNotFound => "Teklif onayı bulunamadı!";
        public static string OfferConfirmAlreadyNotExist => "Teklif onayı zaten mevcut değil!";
        public static string OfferConfirmRemoved => "Teklif onayı silindi!";
        public static string OfferConfirmUpdated => "Teklif onayı güncellendi";

        //ProductBrand mesajları
        public static string ExistingProductBrand => "Ürün markası mevcut";
        public static string ProductBrandAdded => "Ürün markası eklendi";
        public static string ProductBrandNotFound => "Ürün markası bulunamadı!";
        public static string ProductBrandAlreadyNotExist => "Ürün markası zaten mevcut değil!";
        public static string ProductBrandRemoved => "Ürün markası silindi!";
        public static string ProductBrandUpdated => "Ürün markası güncellendi";

        //ProductBrand mesajları
        public static string ExistingUsageStatus => "Kullanım durumu mevcut";
        public static string UsageStatusAdded => "Kullanım durumu eklendi";
        public static string UsageStatusNotFound => "Kullanım durumu bulunamadı!";
        public static string UsageStatusAlreadyNotExist => "Kullanım durumu zaten mevcut değil!";
        public static string UsageStatusRemoved => "Kullanım durumu silindi!";
        public static string UsageStatusUpdated => "Kullanım durumu güncellendi";
    }
}
