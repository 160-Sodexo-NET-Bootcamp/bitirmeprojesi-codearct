using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Business.Abstract;
using Business.Concrete;
using Business.Mapper.AutoMapper;
using Castle.DynamicProxy;
using Core.Caching;
using Core.Caching.Redis;
using Core.Interceptors;
using Core.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DependencyResolvers
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Dependencies
            builder.RegisterType<EfUnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<EfCategoryDal>().As<ICategoryDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfColorDal>().As<IColorDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfOfferDal>().As<IOfferDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfProductBrandDal>().As<IProductBrandDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfProductDal>().As<IProductDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfUsageStatusDal>().As<IUsageStatusDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfUserDal>().As<IUserDal>().InstancePerLifetimeScope();
            builder.RegisterType<EfSentMailDal>().As<ISentMailDal>().InstancePerLifetimeScope();
            builder.RegisterType<TokenHelper>().As<ITokenHelper>().InstancePerLifetimeScope();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<ColorService>().As<IColorService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();
            builder.RegisterType<MailService>().As<IMailService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductBrandService>().As<IProductBrandService>().InstancePerLifetimeScope();
            builder.RegisterType<UsageStatusService>().As<IUsageStatusService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();

            //Redis
            builder.RegisterType<RedisCacheService>().As<ICacheService>().SingleInstance();

            //Interceptor for AOP
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                    .AsImplementedInterfaces()
                    .EnableInterfaceInterceptors(
                        new ProxyGenerationOptions()
                        {
                            Selector = new AspectInterceptorSelector()
                        }
                    )
                    .SingleInstance();

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            })).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .SingleInstance();
        }
    }
}
