using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using SCM.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SCM.Data
{
    interface IGenericRepository<TEntity>
    {
         Task<IList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            bool AsTrackable = true);

        Task<TEntity> GetByIDAsync(object id);
        Task DeleteAsync(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
    }
}