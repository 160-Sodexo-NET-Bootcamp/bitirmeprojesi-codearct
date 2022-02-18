using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation.FluentValidation
{
    public static class ValidationTool
    {
        //Parametre olarak verilen Validator yine parametre olarak verilen modelin validasyon işlemlerini yapar
        public static void Validate(IValidator validator, object model)
        {
            var context = new ValidationContext<object>(model);
            var result = validator.Validate(context);
            //Validasyon başarısız olursa hata fırlatıyor ilgili validasyona göre
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
