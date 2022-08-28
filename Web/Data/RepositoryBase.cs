namespace Web.Data;

public class RepositoryBase<T> : IRepository<T>
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

    public virtual Task<T?> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public  virtual Task Insert(IEnumerable<T> t)
    {
        throw new NotImplementedException();
    }

    public  virtual Task Remove(T t)
    {
        throw new NotImplementedException();
    }

    public  virtual Task Upsert(IEnumerable<T> t)
    {
        throw new NotImplementedException();
    }

    public virtual Task Update(IEnumerable<T> t)
    {
        throw new NotImplementedException();
    }
    
}