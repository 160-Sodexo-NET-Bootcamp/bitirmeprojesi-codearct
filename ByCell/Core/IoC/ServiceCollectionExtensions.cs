using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC
{
    public static class ServiceCollectionExtensions
    {
        //Parametre olarak aldığı servis koleksiyonlarını tek seferde çözen method.
        //Business olmayan her proje de kullanılabilecek servislerimizi startupda ayağa kaldırmamızı sağlar
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection serviceCollection, ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Load(serviceCollection);
            }
            return ServiceTool.Create(serviceCollection);
        }
    }
}
