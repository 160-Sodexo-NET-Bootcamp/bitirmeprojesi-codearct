namespace Core.Results
{
    //Data taşımayan başarısız sonuçlar
    public class ErrorResult : Result
    {
        //Sadece mesaj alabilir
        public ErrorResult(string message) : base(false, message)//Miras aldığı sınıfa false değerini ve mesajı döner
        {

        }
        public ErrorResult() : base(false)//hiç mesaj almazsa miras aldığı sınıfa false değerini döner
        {

        }
    }
}
