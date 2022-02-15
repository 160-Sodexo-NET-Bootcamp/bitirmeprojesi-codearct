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

namespace Business.Security
{
    public class SecuredOperation : MethodInterception
    {
        private string _entityName;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecuredOperation(string entityName)
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            _uow = ServiceTool.ServiceProvider.GetService<IUnitOfWork>();
            _entityName = entityName;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            int userId;
            try
            {
                userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);               
            }
            catch (Exception)
            {
                throw new Exception("Lütfen giriş yapınız!");
            }
            switch (_entityName)
            {
                case "Product":
                    var product = _uow.Products.Get(p => p.UserId == userId);
                    if (product is null)
                    {
                        throw new Exception(Messages.ProductAuthDenied);
                    }
                    break;
                case "Offer":
                    var offer = _uow.Offers.Get(o => o.UserId == userId);
                    if (offer is null)
                    {
                        throw new Exception(Messages.OfferAuthDenied);
                    }
                    break;
                case "User":
                    var user = _uow.Users.Get(p => p.Id == userId);
                    if (user is null)
                    {
                        throw new Exception(Messages.UserNotFound);
                    }
                    break;
                default:
                    break;
            }


        }
    }
}
