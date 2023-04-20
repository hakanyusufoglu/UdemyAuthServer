using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Repositories;

namespace UdemyAuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        //veritabanıyla işlem yapıcağım için;
        private readonly DbContext _context;
        //Tablolar üzerinde işlem yapabilmek için kullanılır ve dbset veritabanı tarafında tablolara denk gelir.
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            //savechange servis katmanında çağıralacak otomatik rollbacki iunitofwork sağlıyor. 
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() //data geldikten sonra üzerinde where sorgusu çalıştırmayacağımdan ıenumrable olarak aldım.
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            //Find async sadece primary key olanları arar.
            var entity = await _dbSet.FindAsync(id);

            //EntityState.Detached yapısını service classını anlatırken detaylandıracak 

            if (entity != null)
            {
                //GetByIdAsync metodunu update veya remove için kullandıktan sonra idye göre getirilen bilginin ramde kalmasını istemiyorum. Ramde takip edilmesin diyorum detached ile.
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified; //burada modified olarak işaretledik. Ancak bir dezavantajı var.
            //Örneğin tek bir kaydın alanını değiştirdiğimizde ve güncellemek istediğimizde ilgili alanın dışında diğer tüm alanlar varolan değerle güncellenir ve bu sonuç performansı olumsuz etkilemektedir.

            //Business kurallarının çok olduğu projelerde domain driven design pattern kullanılır.
            return entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            //IQuaraybble servis katmanında örneğin product geldi where komutunu kullandım order by komutunu kullandım vs. bunları ramde tutulmasını sağlayıp direkt olarak veritabanına işlemesini engellemektedir.
            //ToList() metoduyla birlikte örneğin: product.where(x=>x.id>5).firstdefaukt().tolist() dediğimizde işlemler veritabanına yansır.
            //IEnumurable ise direkt yansıtır ve bu yüzden ıenumarable bir veri geldiğinde üzerine sorgu yazılmaması tavsiye edilir.

            //mümkün olduğunca ıqurayble kullanmalı.
            return _dbSet.Where(predicate);
        }
    }
}
