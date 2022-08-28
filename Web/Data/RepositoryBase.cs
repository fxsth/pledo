using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly DbContext DbContext;

    protected RepositoryBase(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual Task<IEnumerable<T>> GetAll()
    {
        throw new NotImplementedException();
    }
    
    public virtual IEnumerable<T> Get(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split
                     (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return orderBy(query).ToList();
        }
        else
        {
            return query.ToList();
        }
    }

    public virtual async Task<T?> GetById(string id)
    {
        return await DbContext.FindAsync<T>(id);
    }

    public virtual async Task<T?> GetByIdIncludeProperty(string id, Func<T, object> include)
    {
        return await DbContext.FindAsync<T>(id);
    }

    public virtual Task Insert(IEnumerable<T> t)
    {
        throw new NotImplementedException();
    }

    public virtual Task Remove(T t)
    {
        throw new NotImplementedException();
    }

    public virtual Task Upsert(IEnumerable<T> t)
    {
        throw new NotImplementedException();
    }

    public virtual Task Update(IEnumerable<T> t)
    {
        DbContext.UpdateRange(t);

        return Task.CompletedTask;
    }

    public virtual Task Update(T t)
    {
        DbContext.Update(t);
        return Task.CompletedTask;
    }
}