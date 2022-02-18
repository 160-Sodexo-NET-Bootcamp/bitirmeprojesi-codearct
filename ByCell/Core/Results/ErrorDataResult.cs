namespace Core.Results
{
    //Data taşıyan başarısız sonuçlar
    public class ErrorDataResult<T> : DataResult<T>
    {
        //İçine data ve mesaj alabilir
        public ErrorDataResult(T data, string message) : base(data, false, message)
        {

        }
        //İçine sadece data alabilir
        public ErrorDataResult(T data) : base(data, false)
        {

        }
        //İçine sadece mesaj alabilir
        public ErrorDataResult(string message) : base(default, false, message)
        {

        }
        //Hiçbir parametre almayabilir
        public ErrorDataResult() : base(default, false)
        {

        }
    }
}
