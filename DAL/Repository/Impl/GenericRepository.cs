using System.Linq.Expressions;
using DAL.Base;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.Impl;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly FUNewsManagementContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(FUNewsManagementContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>>? include)
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
        {
            query = include.Compile()(query);
        }

        return await query.ToListAsync();
    }
    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<IList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
    {
        query = query.AsNoTracking();
        int count = await query.CountAsync();
        IReadOnlyCollection<T> items = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
        return new BasePaginatedList<T>(items, count, index, pageSize);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> InsertAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        return Task.FromResult(_dbSet.Update(entity));
    }

    public async Task DeleteAsync(object id)
    {
        T entity = await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException();
        _dbSet.Remove(entity);

    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    

    public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync();
        if (entities.Any())
        {
            _dbSet.RemoveRange(entities);
        }
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }
}