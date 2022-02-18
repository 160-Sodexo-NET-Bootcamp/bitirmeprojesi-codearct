using Business.DTOs.OfferDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation.OfferValidation
{
    public class SendOfferValidator: AbstractValidator<SendOfferDto>
    {
        public SendOfferValidator()
        {
            //OfferedPricePercentage boş olursa OfferedPrice boş olamaz
            RuleFor(offer => offer.OfferedPrice).NotEmpty().When(offer => offer.OfferedPricePercentage == default);
            //OfferedPrice boş olursa OfferedPricePercentage boş olamaz
            RuleFor(offer => offer.OfferedPricePercentage).NotEmpty().When(offer => offer.OfferedPrice == default);
        }
    }
}
