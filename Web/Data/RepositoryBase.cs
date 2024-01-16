using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly CustomDbContext CustomDbContext;

    protected RepositoryBase(CustomDbContext customDbContext)
    {
        CustomDbContext = customDbContext;
    }

    public virtual IReadOnlyCollection<T> GetAll()
    {
        return CustomDbContext.Set<T>().AsNoTracking().ToList();
    }

    public virtual async Task<IEnumerable<T>> Get(Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "", int offset = 0, int size = 0)
    {
        IQueryable<T> query = CustomDbContext.Set<T>();

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
            query = orderBy(query);
        }

        query = query.Skip(offset);

        if (size != 0)
            query = query.Take(size);

        return await query.ToListAsync();
    }
    
    public async Task<int> Count(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = CustomDbContext.Set<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync();
    }

    public virtual async Task<T?> GetById(string id)
    {
        return await CustomDbContext.FindAsync<T>(id);
    }

    public virtual async Task<T?> GetByIdIncludeProperty(string id, Func<T, object> include)
    {
        return await CustomDbContext.FindAsync<T>(id);
    }

    public virtual async Task Insert(IEnumerable<T> t)
    {
        await CustomDbContext.AddRangeAsync(t);
    }

    public virtual async Task Insert(T t)
    {
        await CustomDbContext.AddAsync(t);
    }

    public virtual Task Remove(T t)
    {
        CustomDbContext.Remove(t);
        return Task.CompletedTask;
    }

    public virtual Task Remove(IEnumerable<T> t)
    {
        CustomDbContext.RemoveRange(t);
        return Task.CompletedTask;
    }

    public virtual Task Upsert(IEnumerable<T> t)
    {
        var itemsInDb = CustomDbContext.Set<T>().ToList();
        var itemsToUpsert = t.ToList();
        var itemsToDelete = itemsInDb.Where(x => !itemsToUpsert.Contains(x));
        var itemsToInsert = itemsToUpsert.Where(x => !itemsInDb.Contains(x));
        var itemsToUpdate = itemsInDb.Where(x => itemsToUpsert.Contains(x));
        CustomDbContext.RemoveRange(itemsToDelete);
        CustomDbContext.AddRangeAsync(itemsToInsert);
        CustomDbContext.UpdateRange(itemsToUpdate);
        return Task.CompletedTask;
    }

    public virtual Task Update(IEnumerable<T> t)
    {
        CustomDbContext.UpdateRange(t);

        return Task.CompletedTask;
    }

    public virtual Task Update(T t)
    {
        CustomDbContext.Update(t);
        return Task.CompletedTask;
    }
}