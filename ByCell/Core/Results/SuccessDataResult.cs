namespace Core.Results
{
    //Data taşıyan başarılı sonuçlar
    public class SuccessDataResult<T> : DataResult<T>
    {
        //İçine data ve mesaj alabilir
        public SuccessDataResult(T data, string message) : base(data, true, message)
        {

        }
        //İçine sadece data alabilir
        public SuccessDataResult(T data) : base(data, true)
        {

        }
        //İçine sadece mesaj alabilir
        public SuccessDataResult(string message) : base(default, true, message)
        {

        }
        //Hiçbir parametre almayabilir
        public SuccessDataResult() : base(default, true)
        {

        }
    }
}
