using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configuration;
using SharedLibrary.Exceptions;
using SharedLibrary.Extensions;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;
using UdemyAuthServer.Data;
using UdemyAuthServer.Data.Repositories;
using UdemyAuthServer.Service;
using UdemyAuthServer.Service.Services;

namespace UdemyAuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //OptionPattern
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));

            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            //DI Register //auth affect inject ile service k�sm�nda bunlar� tan�mlayabiliriz.
            services.AddScoped<IAuthenticationService, AuthenticationService>(); // IAuthenticationService ile kar��la��ld���nda AuthenticationService 'dan servise �rne�i al.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic b�yle tan�mlan�yor.
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //sql server kullan�ca��m�z� belirtiyoruz.
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions => {

                    //AppDbContextin oldu�u yer yani migrationnun istedi�imiz yeri belirtiyoruz.
                    sqlOptions.MigrationsAssembly("UdemyAuthServer.Data");

                });
            });

            services.AddIdentity<UserApp, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //token �retmek i�in default token provider sa�l�yoruz �ncesinde de entityframework kullanan appdbcontexti al diyoruz



            //K�ML�K DO�RULAMA
            services.AddAuthentication(options =>
            {
                //Alttaki �ema ne i�e yar�yor?
                //�rne�in iki farkl� kullan�c� giri�i oldu�u durumlarda schema kullan�l�r
                //Adminse ayr� urle gidicek �rne�in, normal kullan�c� ayr� giderse bearer kullanmayaca��z
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; ;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>(); // token options art�k dolu olarak geliyor.
                //yukar�daki JwtBearerDefaults.AuthenticationScheme ile buradaki JwtBearerDefaults.AuthenticationSchemeyi konu�turuyoruz sadece bu y�den var bu kod;
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {

                    //appsettings.jsonda var
                    ValidIssuer = tokenOptions.Issuer, //appsettingsde ki issur ile bu issue ayn� m� kontrol edicek
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, //imzas� olacak m�
                    ValidateAudience = true, //validate audince do�rula
                    ValidateIssuer = true, //validate issue do�rula
                    ValidateLifetime = true, // zaman�da kontrol et
                    ClockSkew = TimeSpan.Zero //clockskew �udur. farkl� sunuculara kurmu� oldu�umda serverlarda zaman fark� olabilir ve bu y�zden otomatikman +5 dk ekler. Zeto diyerek hi� bu +5 dkyi ekleme diyorum.

                };

            });














         

            services.AddControllers().AddFluentValidation(options => {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();//startupta bulunan projede AbtastactValidatior olan �eyleri hepsini arar
            });
            services.UseCustomValidationResponse();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UdemyAuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UdemyAuthServer.API v1"));
            }
            else
            {
               

            }

            //s�ralamalar �nemli - Bunlar�n hepsi middleware'dir
            app.UseCustomException();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
