using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Models;

namespace UdemyAuthServer.Data.Configurations
{
    //IEntityTypeConfiguration interfaceni yazmamızın sebebi appdbcontexteki onmodelcreating metodunun içerisine yazıyormuş gibi yapmamızdan kaynaklanmaktadır.
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //x=producttir 
            builder.HasKey(x => x.Id); //primary key yaptık.
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)"); //decimal(18,2) demek toplamda 18 hane olacak. Virgülden önce 16 rakam, virgülden sonrada en fazla 2 rakam tut diyoruz.
            //Örnek: 1000000000000000.00 gibi.
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
