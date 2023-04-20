using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data
{
    // Identity üyelik sistemleri tabloları oluşacak,
    //Aynı zamanda corede ki models içerisindeki entryleri aynı veritabanında tutmak istiyorum. Üyelik sistemi vs ayrı olmasın istiyorum ki performans düşmesin. 
    //Bu yüzden aynı veritabanında tek bir dbcontextte tutacağız
    //üyelik sistemini barındıracağından DbContextten değilde IdentityDbContextten miras alacağım.
   public class AppDbContext : IdentityDbContext<UserApp, IdentityRole,string> 
        //(Tabloları eklerken üyelik sistemi hangisi? onu belirtiyor, İdentityRolünü hazır olarak kullanıyoruz, Primary keyleri nasıl tutayim. best practise göre string tut diyoruz.).
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) //DbContextOption startup tarafında dolduruluyor. Hangi dbcontextte appdbcontexte diyoruz.
        {

        }
        //üyelik için olan dbsetler otomatik eklenecek IdentityDbContexte miras olarak geliyor zaten.
        public DbSet<Product> Products{ get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens{ get; set; }


        //veritabanında oluşan tabloların alanları required olcak m olmucak mı vs belirtiyoruz. (KISITLAMALAR). Bunu configurations klasörü altına ekliyoruz
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //sen bana bir assembly ver ve ben bu assembly içerisinde IEntitypeConfigurationı implement eden tüm classları ekliyim diyor.
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly); //Data içerisinde ki interfaceleri arayıp implement edecek.
            base.OnModelCreating(builder);
        }



    }
}
