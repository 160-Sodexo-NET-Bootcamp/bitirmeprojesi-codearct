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

        [ValidationAspect(typeof(SendOfferValidator))]
        public IResult ChangeOffer(int offerId, SendOfferDto updateOfferDto)
        {
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                        && o.IsActive == true 
                                        && o.ConfirmStatus!=(short)ConfirmStatus.Onaylandı);
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }

            var product = _uow.Products.Get(p => p.Id == offer.ProductId
                                         && p.IsOfferable == true
                                         && p.IsActive == true);
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (offer.UserId != userId)
            {
                return new ErrorResult(Messages.OfferAuthDenied);
            }
            offer.OfferedPrice = updateOfferDto.OfferedPrice == default
                    ? product.Price * updateOfferDto.OfferedPricePercentage / 100
                    : updateOfferDto.OfferedPrice;
            offer.OfferedPricePercentage = updateOfferDto.OfferedPricePercentage;
            offer.ConfirmStatus = (short)ConfirmStatus.Beklemede;

            _uow.Offers.Update(offer);
            _uow.Commit();

            return new SuccessResult(Messages.OfferUpdated);
        }

        public IResult ConfirmOffer(int offerId,bool confirmStatus)
        {
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                    && o.ConfirmStatus==(short)ConfirmStatus.Beklemede
                                    && o.IsActive == true);
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }

            var product = _uow.Products.Get(p => p.Id == offer.ProductId && p.IsActive==true);
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            if (confirmStatus)
            {
                offer.ConfirmStatus = (short)ConfirmStatus.Onaylandı;
                product.IsOfferable = false;
                product.IsSold = true;
                _uow.Products.Update(product);
            }
            else
                offer.ConfirmStatus = (short)ConfirmStatus.Reddedildi;

            _uow.Offers.Update(offer);
            _uow.Commit();

            return new SuccessResult(Messages.OfferConfirmed);
        }

        public IDataResult<List<GetOfferDto>> GetAllByProductId(int productId)
        {
            var product = _uow.Products.Get(p => p.Id == productId && p.IsActive==true);
            if (product is null)
            {
                return new ErrorDataResult<List<GetOfferDto>>(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId!=userId)
            {
                return new ErrorDataResult<List<GetOfferDto>>(Messages.ProductAuthDenied);
            }

            var offers = _uow.Offers.GetAllOffers(o => o.ProductId == productId && o.IsActive==true);
            var offerDtos = _mapper.Map<List<GetOfferDto>>(offers);
            return new SuccessDataResult<List<GetOfferDto>>(offerDtos);
        }

        public IDataResult<List<GetOfferDto>> GetAllByUserId()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _uow.Users.Get(u => u.Id == userId);
            if (user is null)
            {
                return new ErrorDataResult<List<GetOfferDto>>(Messages.UserNotFound);
            }
            var offers = _uow.Offers.GetAllOffers(o => o.UserId == userId && o.IsActive==true);
            var offerDtos = _mapper.Map<List<GetOfferDto>>(offers);
            return new SuccessDataResult<List<GetOfferDto>>(offerDtos);
        }

        public IResult GetBackOffer(int offerId)
        {
            var offer = _uow.Offers.Get(o => o.Id == offerId 
                                     && o.ConfirmStatus!=(short)ConfirmStatus.Onaylandı
                                     && o.IsActive == true);
            if (offer is null)
            {
                return new ErrorResult(Messages.OfferNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (offer.UserId != userId)
            {
                return new ErrorResult(Messages.OfferAuthDenied);
            }

            offer.IsActive = false;

            _uow.Offers.Update(offer);
            _uow.Commit();

            return new ErrorResult(Messages.OfferRemoved);
        }

        public IDataResult<GetOfferDto> GetById(int id)
        {
            var offer = _uow.Offers.GetOffer(o => o.Id == id && o.IsActive==true);
            if (offer is null)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.OfferNotFound);
            }
            var product = _uow.Products.Get(p => p.Id == offer.ProductId);
            if (product is null)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (offer.UserId!=userId || product.UserId!=userId)
            {
                return new ErrorDataResult<GetOfferDto>(Messages.OfferAuthDenied);
            }
            var offerDto = _mapper.Map<GetOfferDto>(offer);
            return new SuccessDataResult<GetOfferDto>(offerDto);
        }

        [ValidationAspect(typeof(SendOfferValidator))]
        public IResult SendOffer(int productId, SendOfferDto createOfferDto)
        {
            var product = _uow.Products.Get(p => p.Id == productId && p.IsOfferable==true);
            if (product is null)
            {
                return new ErrorResult(Messages.OfferProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId==userId)
            {
                return new ErrorResult(Messages.NotOfferOwnProduct);
            }

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
