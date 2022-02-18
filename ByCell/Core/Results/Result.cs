using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Results
{
    //İçinde data bulundurmayan her sonuc(başarılı-başarısız)
    public class Result : IResult
    {
        //İçine mesaj ve başarı durumunu alabilir
        public Result(bool success, string message) : this(success)//Kendi methoduna başarı durumunu döner
                                                                   //Success propertisine ulaşılabilir.
        {
            Message = message;
        }
        //İçine sadece başarı durumunu alabilir
        public Result(bool success)
        {
            Success = success;
        }
        public bool Success { get; }
        public string Message { get; }
    }
}
