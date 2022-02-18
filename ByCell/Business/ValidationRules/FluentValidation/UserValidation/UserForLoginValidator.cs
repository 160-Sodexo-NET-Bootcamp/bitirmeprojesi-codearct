using Business.DTOs.UserDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation.UserValidation
{
    public class UserForLoginValidator : AbstractValidator<UserForLoginDto>
    {
        public UserForLoginValidator()
        {
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email boş bırakılamaz!");
            RuleFor(user => user.Email).EmailAddress().WithMessage("Geçerli bir mail adresi giriniz!");
            RuleFor(user => user.Password).NotEmpty().WithMessage("Şifre giriniz!");
            RuleFor(user => user.Password).MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalı!");
            RuleFor(user => user.Password).MaximumLength(20).WithMessage("Şifre en fazla 20 karakter olmalı!");
        }
    }
}
