using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace Core.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            //İlgili imzayı taşıyan bütün sınıf üzerinde bulunan attribute methodları yakalar
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();

            //İlgili imzayı taşıyan bütün method üzerinde bulunan attribute methodları yakalar
            var methodAttributes = type.GetMethod(method.Name)
                                       .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            //Method üzerindekileri sınıf üzerinde yakaldığı listeye atar
            classAttributes.AddRange(methodAttributes);

            //Önem sırasına göre düzenler
            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
