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
            RuleFor(user => user.FirstName).NotEmpty();
            RuleFor(user => user.LastName).NotEmpty();
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email boş bırakılamaz!");
            RuleFor(user => user.Email).EmailAddress().WithMessage("Geçerli bir mail adresi giriniz!");
            RuleFor(user => user.Password).NotEmpty();
            RuleFor(user => user.Password).MinimumLength(8);
            RuleFor(user => user.Password).MaximumLength(20);
        }
    }
}
