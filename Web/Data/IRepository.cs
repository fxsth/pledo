namespace Web.Data;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(string id);
    Task Insert(IEnumerable<T> t);
    Task Remove(IEnumerable<T> t);
    Task Upsert(IEnumerable<T> t);
    Task Update(IEnumerable<T> t);
}