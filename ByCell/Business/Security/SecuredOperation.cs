using Business.Constants;
using Castle.DynamicProxy;
using Core.Interceptors;
using Core.IoC;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Security;

namespace Business.Security
{
    //Interceptor for AOP
    public class SecuredOperation : MethodInterception
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation()
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        //İsteği yapan kullanıcı giriş yapmış mı kontrolü yapar
        protected override void OnBefore(IInvocation invocation)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new SecurityException(Messages.AuthorizationsDenied);
            }
        }
    }
}
