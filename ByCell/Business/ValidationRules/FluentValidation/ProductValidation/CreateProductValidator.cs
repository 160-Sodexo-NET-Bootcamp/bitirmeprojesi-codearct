using Business.DTOs.ProductDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation.ProductValidation
{
    public class CreateProductValidator:AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(product => product.Name).NotEmpty().WithMessage("Ad alanı boş bırakılamaz!");
            RuleFor(product => product.Name).MaximumLength(100);
            RuleFor(product => product.Description).NotEmpty().WithMessage("Tanıtım alanı boş bırakılamaz!");
            RuleFor(product => product.Description).MaximumLength(500);
            RuleFor(product => product.CategoryId).GreaterThan(0).WithMessage("Bir kategori seçiniz!");
            RuleFor(product => product.UsageStatusId).GreaterThan(0).WithMessage("Ürün kullanım durumunu seçiniz!");
            RuleFor(product => product.Price).GreaterThan(0).WithMessage("Bir fiyat giriniz!");
        }
    }
}
