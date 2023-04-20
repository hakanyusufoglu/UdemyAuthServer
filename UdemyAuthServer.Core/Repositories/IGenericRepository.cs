using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Repositories
{
   public interface IGenericRepository<TEntity> where TEntity:class     //Entityler için yapılan en genel işlemler (crud)
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync(); //IEnumurable kullanmamızın sebebi ben tüm modeli çektim ve bu model üzerinden daha fazla where sorgusu vs yazmayacağımı gösteriyorum.



        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate); //FUNC = DELEGE entity alacak geriye bool dönecek     where(x=>x.id>5= x kısmı tentity x.id>5 bool kısmını temsil etmektedir.



        
        //buradaki where komutunu kullandığımızı varsayalım
        //product = productRepository.where(x=>x.id>5) eğer ıquarayble dönüyorsa bu sorgu veritabanına daha yansıtılmadı.

        //where üzerine any metodunu kullanabilirsin product.any

        //product.tolist() dediğimizde işlemlerimiz direk olarak veritabanına yansımaktadır. Kısaca yukarıda yazılan sorgular tek bir sorguya çevrilir ve tolist dediğimizde veritabanına o sorgu hali yansır ve bir sonuç döner.


        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        TEntity Update(TEntity entity);

        // _context.Entry(entity).state = EntityState.Modified //entitynin durumunu değiştiriyorum bu yüzden removeda ve update async yok.
    }
}
