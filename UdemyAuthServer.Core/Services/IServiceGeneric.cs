using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Services
{

    //hem tentity hem döneceği dto tipini alıyor
   public interface IServiceGeneric<TEntity,TDto> where TEntity:class where TDto:class 
    {
        //Tentity Tdtoya dönüşecek ve bu yüzden tdto koyduk
        //DİREKT Apinin kullanacağı datayı döneceğiz
        //responseda datada dönüyor o yüzden kullandıkç
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync(); 
      Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); //ıquarayble ıenumrarable yapmamızın sebebi artık istediğimiz data gelecek bize yani data katmanında id>5 yaptıysak bize id>5 verileri geleceğinden ienumarable yaptık. Eğer ki daha başka sorgu yazacaksak ıenumarable gerekli
        Task<Response<TDto>> AddAsync(TDto entity);
        Task<Response<NoDataDto>> Remove(int id); //geriye normalde bir şey dönmüyor ama burada sharedlibraryıda nodatadtoyu yani boş datayı döndüreceğiz
        Task<Response<NoDataDto>> Update(TDto entity,int id);

    }
}
