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
        var items = t.ToList();
        foreach (var item in items)
        {
            DbContext.Update(item);
        }
        return Task.CompletedTask;
    }
}