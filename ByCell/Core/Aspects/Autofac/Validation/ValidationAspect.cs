using Castle.DynamicProxy;
using Core.Aspects.Autofac.Validation.FluentValidation;
using Core.Interceptors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect : MethodInterception
    {
        private Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            //Method başında verilen Type bir Validator type değilse hata dönüyor
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new Exception("Bu bir doğrulama değil!");
            }

            _validatorType = validatorType;
        }
        //İlgili methodun başında çalışmaya başlıyacak
        protected override void OnBefore(IInvocation invocation)
        {
            //Verilen Validator için bir instance oluşturacak
            var validator = (IValidator)Activator.CreateInstance(_validatorType);
            //Validator'ın validasyon modelini yakalıcak
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];
            //Bu yakaladığı modeli methodumuzun(invocation) parametrelerinde arayacak
            var entities = invocation.Arguments.Where(t => t.GetType() == entityType);
            //Bulduğu modellerin validasyonunu yapacak
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}
