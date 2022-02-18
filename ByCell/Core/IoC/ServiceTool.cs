using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC
{
    //Verilen servis koleksiyonlarını hizmete sunar
    public static class ServiceTool
    {
        public static IServiceProvider ServiceProvider { get; private set; }
       
        public static IServiceCollection Create(IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            return services;
        }
    }
}
