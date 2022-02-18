using Business.DTOs.ProductDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation.ProductValidation
{
    public class UpdateProductValidator: AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            //Name alanı boş değilse en fazla 100 karakter uzunluğunda olmalı
            RuleFor(product => product.Name).MaximumLength(100)
                .When(p=>!string.IsNullOrEmpty(p.Name))
                .WithMessage("En fazla 100 karakter girilebilir!");

            //Description alanı boş değilse en fazla 500 karakter uzunluğunda olmalı
            RuleFor(product => product.Description).MaximumLength(500)
                .When(p => !string.IsNullOrEmpty(p.Description))
                .WithMessage("En fazla 100 karakter girilebilir!"); ;
        }
    }
}
