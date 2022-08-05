using System.Linq.Expressions;

namespace PriceHunter.Common.Data.Abstract
{
    public interface IGenericRepository<TEntity>
    {
        Task<TEntity> InsertAsync(TEntity entity);
        Task<List<TEntity>> InsertManyAsync(List<TEntity> entityList);
        
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<List<TEntity>> UpdateManyAsync(List<TEntity> entityList);

        Task DeleteAsync(TEntity entity);
        Task DeleteManyAsync(List<TEntity> entityList);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate);
        
        Task BulkInsertAsync(List<TEntity> entities);
        void BulkInsert(List<TEntity> entities); 
    }
}