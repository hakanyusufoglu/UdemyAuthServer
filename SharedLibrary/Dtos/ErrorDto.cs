using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
   public class ErrorDto
    {
        //Tüm projelerde aynı error tipi olacağından bunu errordtoyu sharedlibrarye ekledik.
        //birden fazla hata meydana gelebilir.
        public List<String> Errors { get; private set; } = new List<string>();//sadece bu class içerisinde bu propertyler set edilsin.

        //private set olmasaydı yani sadece get olsaydı bir metotta bunu set edemeyecektik.
        public bool IsShow { get; private set; } // mobile veya web uygulamada olabilir. Gelen hata kullanıcıya gösterilmesini sağlar. Sadece yazılımcının anlayacağı bir hata ile karşılaşmışsak IsShow=false yapar kullanıcıya göstermeyiz.
    
        public ErrorDto(string error, bool isShow) //eğer sadece 1 adet hata gelirse burayı çağırcak
        {
            Errors.Add(error);
            IsShow = isShow; //eski hali isShow=true idi.
        }
        public ErrorDto(List<string> errors, bool isShow) //Birden fazla hata gelirse buraya gelecek
        {
            Errors = errors;
            IsShow = isShow;
        }
    }
}
