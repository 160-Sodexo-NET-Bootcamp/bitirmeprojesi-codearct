using Business.DTOs.OfferDTOs;
using Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IOfferService
    {
        IDataResult<List<GetOfferDto>> GetAllByProductId(int productId);
        IDataResult<List<GetOfferDto>> GetAllByUserId();//UserId HttpContext den alınacak
        IDataResult<GetOfferDto> GetById(int id);
        IResult SendOffer(int productId,SendOfferDto createOfferDto);
        IResult ChangeOffer(int offerId,SendOfferDto updateOfferDto);
        IResult ConfirmOffer(bool confirmStatus);
        IResult GetBackOffer(int offerId);
    }
}
