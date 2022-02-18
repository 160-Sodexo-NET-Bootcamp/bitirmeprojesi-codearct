using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interceptors
{
    //Bu imzaya sahip bütün sınıf ve  methodları attribute haline dönüştürür
    //Sınıf ve methodun başlarına eklenebilir;birden fazla kullanılabilir ve miras alınabilir
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class MethodInterceptionBaseAttribute : Attribute, IInterceptor
    {
        //Attribute ün önem sırasını belirler
        public int Priority { get; set; }

        //Ezilebilir intercept methodu
        public virtual void Intercept(IInvocation invocation)
        {

        }
    }
}
