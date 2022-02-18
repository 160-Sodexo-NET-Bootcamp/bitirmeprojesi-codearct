namespace Core.Results
{
    //Data taşımayan başarılı sonuç
    public class SuccessResult : Result
    {
        //İçine mesaj alabilir
        public SuccessResult(string message) : base(true, message)//Başarılı sonuç olduğu için
                                                                  //miras aldığı sınıfa mesajını ve true değerini döner
        {

        }
        public SuccessResult() : base(true)//Hiç mesaj almazsa sadece miras aldığı sınıfa true döner
        {

        }
    }
}
