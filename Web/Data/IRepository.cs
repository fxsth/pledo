namespace Web.Data;

public interface IRepository<T> where T : class
{
    IReadOnlyCollection<T> GetAll();
    Task<T?> GetById(string id);
    Task Insert(T t);
    Task Insert(IEnumerable<T> t);
    Task Remove(T t);
    Task Remove(IEnumerable<T> t);
    Task Upsert(IEnumerable<T> t);
    Task Update(IEnumerable<T> t);
}