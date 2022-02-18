using Core.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging.Middleware
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICustomLogger _logger;
        public CustomExceptionMiddleware(RequestDelegate next, ICustomLogger logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            //Zaman sayacı middleware çalışmaya başladığında çalıştırılır
            var watch = Stopwatch.StartNew();
            //Bütün sistem try-catch bloğu içine middleware vasıtasıyla alınır
            //Exceptionlar yakalanır
            //Konsola loglanır
            try
            {
                //Request mesajı oluşturulur
                string message = "[Request]  HTTP " 
                    + context.Request.Method 
                    + " - " 
                    + context.Request.Path;
                //Konsola basılır.
                _logger.Write(message);

                await _next(context);//Bu noktada sistemdeki pipelinedaki diğer metoda geçilir
                //İstek tamamlandığında sayaç durdurulur
                watch.Stop();

                //Başarılı Response mesajı oluşturulur
                message = "[Response] HTTP " 
                    + context.Request.Method 
                    + " - " 
                    + context.Request.Path 
                    + " responded " 
                    + context.Response.StatusCode 
                    + " in " 
                    + watch.Elapsed.TotalMilliseconds + " ms";
                //Mesaj konsola basılır
                _logger.Write(message);
            }
            catch (Exception ex)
            {
                //Request-response hattında herhangi bir exception olursa sayaç durdurulur
                watch.Stop();
                //ilgili exception konsola basılır
                await HandleException(context, ex, watch);
            }

        }

        private Task HandleException(HttpContext context, Exception ex, Stopwatch watch)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Response error mesajı oluşturulur
            string message = "[Error]    HTTP " 
                + context.Request.Method 
                + " - " 
                + context.Response.StatusCode 
                + " Error Message: " 
                + ex.Message + 
                " in " 
                + watch.Elapsed.TotalMilliseconds + " ms";
            //Mesaj konsola basılır
            _logger.Write(message);

            //exception mesajları serialize edilir
            var result = JsonConvert.SerializeObject(new ErrorResult(ex.Message), Formatting.None);
            //excepton mesajı body e geçilir
            return context.Response.WriteAsync(result);
        }
    }
}
