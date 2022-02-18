using Castle.DynamicProxy;
using System;

namespace Core.Interceptors
{
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { }//Method çalışmadan önce başında devreye girsin istediğimizde
        protected virtual void OnAfter(IInvocation invocation) { }//İlgili method işlemini tamamladıktan sonra
        protected virtual void OnException(IInvocation invocation, System.Exception e) { }//İlgili method hata fırlattığında
        protected virtual void OnSuccess(IInvocation invocation) { }//İlgili method başarılı olduğunda
        //Ezilen intercept methodu
        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            //İlgili methodun önünde verilen atribute method çalıştırılır
            OnBefore(invocation);
            try
            {
                invocation.Proceed();//ilgili method çalıştırılır
            }
            catch (Exception e)
            {
                isSuccess = false;
                //Attribute method hata fırlatıldığında çalıştırılır
                OnException(invocation, e);
                throw;
            }
            finally
            {
                //Attribute method ilgili method başarılı sonuç döndüğünde çalıştırılır
                if (isSuccess)
                {
                    OnSuccess(invocation);
                }
            }
            //ilgili method bittikten sonra attribute method çalıştırılır
            OnAfter(invocation);
        }
    }
}
