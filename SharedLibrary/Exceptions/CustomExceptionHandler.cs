using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
   public static class CustomExceptionHandler
    {
        //startup içerisindeki configure metodu için yazıyoruz
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config=> {
                //sonlandırıcı middleware bu aşamadan sonra bir sonraki middlewarea geçmez //Use kullanırsak devam edicidir.
                config.Run(async context=> {

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    //interface üzerinden hataları yakalayacağız
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(errorFeature!=null)
                    {
                        var ex = errorFeature.Error;
                        ErrorDto errorDto = null;
                        if(ex is CustomException) //bizim gördüğümüz hata mesajı
                        {
                            errorDto = new ErrorDto(ex.Message, true);
                        }
                        else //500 dönen bir hata ise 
                        {
                            errorDto = new ErrorDto(ex.Message, false);
                        }

                        var response = Response<NoDataDto>.Fail(errorDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                
                });
            
            });//uygulamalardaki tüm hatayı yakalayan middleware
        }
    }
}
