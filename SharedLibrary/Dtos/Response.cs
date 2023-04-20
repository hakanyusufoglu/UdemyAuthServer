using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{ //Niçin kullanıyoruz?
  //Clientlara verilecek olan modelleri direkt olarak response<T> olarak sağlıyoruz yani reponse<Product> dediğimizde kullanıcıya product tipten bir response gelecek.
    public class Response<T> where T : class
    {
        //BEST PRACTISE'e göre uptade ve remove'da geriye bir değer döndürülmemeli. 
        public T Data { get; private set; } //T tipinde bir data tutacağız ve bu sınıf içinde set yapılacak
        public int StatusCode { get; private set; } //200, 204 vs

        //CLIENTLARA AÇMAYACAĞIM BİR PROPERTY ve JsonIgnore sayesinde json tarafından görülmeyecek (yok sayılcak)
        [JsonIgnore]
        public bool IsSuccessful { get; private set; } //api projelerinde kullanmak istiyorum. Developer direk statusCodedan işin başarılı olacağından bakmasında direkt olarak başarılı mı değil mi diye dönsün. Hiç statuskodun 200 mü 400 mü vs döndüğüne bakılmaksızın başarısızsa sırasıyla true veya false dönerek işi kısalatacak.

        public ErrorDto Error { get; private set; } //kendi içerisinde tek bir hata veya birden fazla hata tutabilir.
        public static Response<T> Success(T data, int statusCode)//Başarılıysa; //nesne oluşturmadan direkt olarak sınıf ismiyle bu response kullanabilmemiz sağlanıyor.
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful=true}; //geriye data göndericek
        }
        public static Response<T> Success(int statusCode)// başarılı dönsede data almayacağım çünkü remove vs nocontent dönecek
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful=true }; //default her ne ise o gelsin.
        }
        //başarısızsa ve birden fazla hata dönerse;
        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }
        //başarısızsa ve tek hata dönerse;
        public static Response<T> Fail(string errorMessage, int statusCode,bool isShow)
        {
            //yukarıdakinden farkı yukarıda nesne oluşturmamız gerekiyor burada gerekmiyor.
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful=false };
        }
    }
}
