using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{

    //exteansions mettotlar statik olmalıdır.
    public static class CustomTokeAuth
    {

        //startup içerisinde configuration metoduna eklemek için. 
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenOption tokenOptions)
        {
            //KİMLİK DOĞRULAMA
            services.AddAuthentication(options =>
            {
                //Alttaki şema ne işe yarıyor?
                //Örneğin iki farklı kullanıcı girişi olduğu durumlarda schema kullanılır
                //Adminse ayrı urle gidicek örneğin, normal kullanıcı ayrı giderse bearer kullanmayacağız
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; ;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                //yukarıdaki JwtBearerDefaults.AuthenticationScheme ile buradaki JwtBearerDefaults.AuthenticationSchemeyi konuşturuyoruz sadece bu yüden var bu kod;
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {

                    //appsettings.jsonda var
                    ValidIssuer = tokenOptions.Issuer, //appsettingsde ki issur ile bu issue aynı mı kontrol edicek
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, //imzası olacak mı
                    ValidateAudience = true, //validate audince doğrula
                    ValidateIssuer = true, //validate issue doğrula
                    ValidateLifetime = true, // zamanıda kontrol et
                    ClockSkew = TimeSpan.Zero //clockskew şudur. farklı sunuculara kurmuş olduğumda serverlarda zaman farkı olabilir ve bu yüzden otomatikman +5 dk ekler. Zeto diyerek hiç bu +5 dkyi ekleme diyorum.

                };

            });

        }
    }
}
