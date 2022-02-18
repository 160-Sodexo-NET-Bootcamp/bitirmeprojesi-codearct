using AutoMapper;
using Business.Abstract;
using Business.Constants;
using Business.DTOs.OfferDTOs;
using Business.Enums;
using Business.Security;
using Business.ValidationRules.FluentValidation.OfferValidation;
using Core.Aspects.Autofac.Validation;
using Core.Results;
using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    //Tüm Teklif sınıf metotları için yetkilendirme kontrolü yapılır
    [SecuredOperation(Priority =1)]
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public OfferService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        //Metot öncesi girilen Dto nun validasyonları yapılır
        [ValidationAspect(typeof(SendOfferValidator))]
        //Daha önce verilen teklif için yeni bir teklif oluşturma
        public IResult ChangeOffer(int offerId, SendOfferDto updateOfferDto)
        {
            //verilen id ye göre silinmemiş ve henüz onaylanmamış teklifler arasından teklif aranır
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                        && o.IsActive == true 
                                        && o.ConfirmStatus!=(short)ConfirmStatus.Onaylandı);
            //Teklif bulunamazsa ilgili mesaj kullanıcıya dönülür
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }
            //Teklif varsa ilgili teklifin hangi ürüne verildiği 
            //ürünler arasından ürün silinmemiş ve hala tekliflere açıksa aranır
            var product = _uow.Products.Get(p => p.Id == offer.ProductId
                                         && p.IsOfferable == true
                                         && p.IsActive == true);
            //Ürün bulunamazsa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            //Token içinde gelen ilgili isteği yapan kullanıcının Id si alınır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Teklifin sahibi bu isteği yapan kullanıcı değilse ilgili hata kullanıcıya dönülür
            if (offer.UserId != userId)
            {
                return new ErrorResult(Messages.OfferAuthDenied);
            }
            //Tüm iş kuralarından geçen istek yeni bir teklif oluşturur
            offer.OfferedPrice = updateOfferDto.OfferedPrice == default
                    ? product.Price * updateOfferDto.OfferedPricePercentage / 100
                    : updateOfferDto.OfferedPrice;
            offer.OfferedPricePercentage = updateOfferDto.OfferedPricePercentage;
            offer.ConfirmStatus = (short)ConfirmStatus.Beklemede;

            _uow.Offers.Update(offer);
            _uow.Commit();

            return new SuccessResult(Messages.OfferUpdated);
        }

        //Teklif onaylanır
        public IResult ConfirmOffer(int offerId,bool confirmStatus)
        {
            //Onaylanacak teklif verilen ıd ye göre silinmemiş ve statüsü "Beklemede" olan teklif arasında aranır
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                    && o.ConfirmStatus==(short)ConfirmStatus.Beklemede
                                    && o.IsActive == true);
            //Teklif yoksa ilgili mesaj kullanıcıya dönülür
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }
            //Varsa ilgili teklifin ürünü silinememişse veritabanında aranır
            var product = _uow.Products.Get(p => p.Id == offer.ProductId && p.IsActive==true);
            //Ürün yoksa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }
            //Ürün varsa ilgili ürünün sahibi isteği yapan kullanıcı mı kontrolü yapılır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //İsteği yapan kullanıcı ürünün sahibi değilse ilgili hata mesajı kullancıya dönülür
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            //Confirmstatüs true ise yani teklif onaylanmışsa ürün satılır tekliflere kapanır
            //Teklifin confirmstatüsü "Onaylandı" olarak değiştirilir
            if (confirmStatus)
            {
                offer.ConfirmStatus = (short)ConfirmStatus.Onaylandı;
                product.IsOfferable = false;
                product.IsSold = true;
                _uow.Products.Update(product);
            }
            //kullanıcıdan gelen confirstatüs değeri false ise
            //Teklifin confirmstatüs değeri "Reddedildi" olarak değiştirilir
            else
                offer.ConfirmStatus = (short)ConfirmStatus.Reddedildi;

            _uow.Offers.Update(offer);
            //ilgili değişiklikler(ürün ve teklifteki) veritabanına kaydeddilir
            _uow.Commit();

            return new SuccessResult(Messages.OfferConfirmed);
        }

        //Kullanıcı kendisine ait ürüne gelen bütün teklifleri listeler
        public IDataResult<List<GetOfferDto>> GetAllByProductId(int productId)
        {
            //Gelen ürün Id sine göre silinemeyn ürünlerde aranır
            var product = _uow.Products.Get(p => p.Id == productId && p.IsActive==true);
            //Ürün yoksa ilgi hata mesajını kullanıcıya dön
            if (product is null)
            {
                return new ErrorDataResult<List<GetOfferDto>>(Messages.ProductNotFound);
            }
            //Ürün varsa isteği yapan kullanıcı ürün sahibimi kontrolü yapılır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //İsteği yapan kullanıcı ürün sahibi değilse ilgili hata mesajı kullanıcıya dönülür
            if (product.UserId!=userId)
            {
                return new ErrorDataResult<List<GetOfferDto>>(Messages.ProductAuthDenied);
            }
            //Tüm iş kurallarından geçen kullanıcının ürün için teklifleri eğer silinmemişse veritabanından çekilir
            var offers = _uow.Offers.GetAllOffers(o => o.ProductId == productId && o.IsActive==true);
            //Automapper ile kullanıcıya gösterilecek formata dönüştürülür
            var offerDtos = _mapper.Map<List<GetOfferDto>>(offers);
            return new SuccessDataResult<List<GetOfferDto>>(offerDtos);
        }

        //Kullanıcı kendisinin başkasının ürünlerine verdiği tüm teklifleri listeleyebilir
        public IDataResult<List<GetOfferDto>> GetAllByUserId()
        {
            //Token üzerinden isteği yapan kullanıcının id si alınır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //İsteği yapan kullanıcının Id si ile veritabanında kayıtlı kullanıcı bulunur
            var user = _uow.Users.Get(u => u.Id == userId);
            //İlgili kullanıcı Id sine göre silinmemiş teklifler veritabanından getirilir
            var offers = _uow.Offers.GetAllOffers(o => o.UserId == userId && o.IsActive==true);
            //Automapper ile kulanıcıya gösterilecek formata dönüştürülür
            var offerDtos = _mapper.Map<List<GetOfferDto>>(offers);
            return new SuccessDataResult<List<GetOfferDto>>(offerDtos);
        }

        //Kullanıcı verdiği teklifi geri çekebilir
        public IResult GetBackOffer(int offerId)
        {
            //Verilen teklif Id sine göre silinmemiş ve statüsü onaylanmamış teklifler arasında arama yapılır
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                     && o.ConfirmStatus!=(short)ConfirmStatus.Onaylandı
                                     && o.IsActive == true);
            //Teklif yoksa kullancıya ilgili hata mesajı dönülür
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }
            //Teklif varsa isteği yapan kullanıcıya ait bir teklif mi kontrolü yapılır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Teklif kullancıya ait değilse ilgili hata mesajı dönülür
            if (offer.UserId != userId)
            {
                return new ErrorResult(Messages.OfferAuthDenied);
            }
            //Tüm iş kurallarından geçen istek teklifi soft olarak veritababnından siler
            offer.IsActive = false;

            _uow.Offers.Update(offer);
            _uow.Commit();

            return new ErrorResult(Messages.OfferRemoved);
        }

        //Teklifin detaylarına ulaşılır
        public IDataResult<GetOfferDto> GetById(int id)
        {
            //Verilen teklif Id sine göre silinmemiş teklifler arasında aranır
            var offer = _uow.Offers.GetOffer(o => o.Id == id && o.IsActive==true);
            //Teklif yoksa ilgili hata mesajı kullanıcıya dönülür
            if (offer is null)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.OfferNotFound);
            }
            //Teklif varsa teklif yapılan ürün silinmemiş ürünler arasında veritabanında aranır
            var product = _uow.Products.Get(p => p.Id == offer.ProductId && p.IsActive==true);
            //Ürün yoksa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.ProductNotFound);
            }
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //İsteği yapan kullanıcı ürünün veya teklifin sahibi mi kontrolü yapılır
            //bu iki kullanıcıdan biri değilse ilgili hata mesajı kullanıcıya dönülür
            if (offer.UserId!=userId || product.UserId!=userId)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.OfferAuthDenied);
            }
            //Tüm iş kurallarımnı geçen Teklif Automapper ile formatlanır
            var offerDto = _mapper.Map<GetOfferDto>(offer);
            return new SuccessDataResult<GetOfferDto>(offerDto);
        }

        //Metot öncesi kullanıcıdan gelen verilerin validasyon kontrolü yapılır
        [ValidationAspect(typeof(SendOfferValidator))]
        //Ürüne teklif yapılır
        public IResult SendOffer(int productId, SendOfferDto createOfferDto)
        {
            //Verilen ürün Id sine göre teklife açık olan ürünler(teklife açıksa ürün silinmemiş demek) arasında aranır 
            var product = _uow.Products.Get(p => p.Id == productId && p.IsOfferable==true);
            //Ürün yoksa kullanıcıya ilgili hata mesajı dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.OfferProductNotFound);
            }
            //Teklifin sahibi kendi ürününe teklif vermeye çalışıyorsa ilgili hata mesajı kullanıcıya dönülür
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId==userId)
            {
                return new ErrorResult(Messages.NotOfferOwnProduct);
            }

            //İş kurallarından geçilirse verilen Dto bilgilerine göre teklif oluşturulur
            var offer = new Offer
            {
                OfferedPrice = createOfferDto.OfferedPrice == default
                    ? product.Price * createOfferDto.OfferedPricePercentage / 100
                    : createOfferDto.OfferedPrice,
                OfferedPricePercentage = createOfferDto.OfferedPricePercentage,
                UserId = userId,
                ProductId = product.Id,
                ConfirmStatus = (short)ConfirmStatus.Beklemede,
                IsActive = true
            };
            _uow.Offers.Add(offer);
            _uow.Commit();
            return new SuccessResult(Messages.OfferAdded);
        }
    }
}
