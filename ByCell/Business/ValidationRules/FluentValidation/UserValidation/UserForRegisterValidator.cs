using Business.DTOs.UserDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation.UserValidation
{
    public class UserForRegisterValidator:AbstractValidator<UserForRegisterDto>
    {
        public UserForRegisterValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().WithMessage("İsim alanı boş bırakılamaz!"); ;
            RuleFor(user => user.LastName).NotEmpty().WithMessage("Soyisim alanı boş bırakılamaz!"); ;
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email boş bırakılamaz!");
            RuleFor(user => user.Email).EmailAddress().WithMessage("Geçerli bir mail adresi giriniz!");
            RuleFor(user => user.Password).NotEmpty().WithMessage("Bir şifre giriniz!"); ;
            RuleFor(user => user.Password).MinimumLength(8).WithMessage("Şifre en az 8 karakterden oluşmalı!"); ;
            RuleFor(user => user.Password).MaximumLength(20).WithMessage("Şifre en fazla 20 karakterden oluşmalı!"); ;
        }
    }
}
