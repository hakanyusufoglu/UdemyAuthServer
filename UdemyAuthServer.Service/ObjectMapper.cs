using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Service
{
    //buradan nesne örneği alacağız.
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() => {

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();
        
        });//lazyloading = ihtiyaç olduğu an rame yükle bu olmasa proje çalışır çalışmaz yüklenecek.

           //lazy metodu çağırmak için;
        public static IMapper Mapper => lazy.Value; //get ve seti yapıyoruz şuan set yok geti oluşturuyoruz


    }
}
