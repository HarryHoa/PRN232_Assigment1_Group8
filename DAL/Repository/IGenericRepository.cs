using System.Linq.Expressions;
using DAL.Base;

namespace DAL.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate);
    Task<IList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include);
    Task<IList<T>> GetAllAsync();
    Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);

    Task<T?> GetByIdAsync(object id);
    Task<T> InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task DeleteAsync(Expression<Func<T, bool>> predicate);

    Task SaveAsync();
    
}