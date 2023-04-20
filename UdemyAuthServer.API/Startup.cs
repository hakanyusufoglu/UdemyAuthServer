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

            //DI Register //auth affect inject ile service kýsmýnda bunlarý tanýmlayabiliriz.
            services.AddScoped<IAuthenticationService, AuthenticationService>(); // IAuthenticationService ile karþýlaþýldýðýnda AuthenticationService 'dan servise örneði al.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //generic böyle tanýmlanýyor.
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                //sql server kullanýcaðýmýzý belirtiyoruz.
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions => {

                    //AppDbContextin olduðu yer yani migrationnun istediðimiz yeri belirtiyoruz.
                    sqlOptions.MigrationsAssembly("UdemyAuthServer.Data");

                });
            });

            services.AddIdentity<UserApp, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //token üretmek için default token provider saðlýyoruz öncesinde de entityframework kullanan appdbcontexti al diyoruz



            //KÝMLÝK DOÐRULAMA
            services.AddAuthentication(options =>
            {
                //Alttaki þema ne iþe yarýyor?
                //Örneðin iki farklý kullanýcý giriþi olduðu durumlarda schema kullanýlýr
                //Adminse ayrý urle gidicek örneðin, normal kullanýcý ayrý giderse bearer kullanmayacaðýz
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; ;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts => {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>(); // token options artýk dolu olarak geliyor.
                //yukarýdaki JwtBearerDefaults.AuthenticationScheme ile buradaki JwtBearerDefaults.AuthenticationSchemeyi konuþturuyoruz sadece bu yüden var bu kod;
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {

                    //appsettings.jsonda var
                    ValidIssuer = tokenOptions.Issuer, //appsettingsde ki issur ile bu issue ayný mý kontrol edicek
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true, //imzasý olacak mý
                    ValidateAudience = true, //validate audince doðrula
                    ValidateIssuer = true, //validate issue doðrula
                    ValidateLifetime = true, // zamanýda kontrol et
                    ClockSkew = TimeSpan.Zero //clockskew þudur. farklý sunuculara kurmuþ olduðumda serverlarda zaman farký olabilir ve bu yüzden otomatikman +5 dk ekler. Zeto diyerek hiç bu +5 dkyi ekleme diyorum.

                };

            });














         

            services.AddControllers().AddFluentValidation(options => {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();//startupta bulunan projede AbtastactValidatior olan þeyleri hepsini arar
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

            //sýralamalar önemli - Bunlarýn hepsi middleware'dir
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
