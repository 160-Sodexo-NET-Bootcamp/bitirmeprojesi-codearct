namespace Core.Results
{
    //Data taşıyan her sonuç(başarılı-başarısız)
    public class DataResult<T> : Result, IDataResult<T>
    {
        //İçine data,başarı durumu ve mesajını alabilir
        public DataResult(T data, bool success, string message) : base(success, message)//Miras aldığı sınıfa başarı durumu ve mesajını dööner
        {
            Data = data;
        }
        //İçine data ve başarı durumunu alır
        public DataResult(T data, bool success) : base(success)//Miraz aldığı sınıfa başarı durumunu döner
        {
            Data = data;
        }

        public T Data { get; }
    }
}
