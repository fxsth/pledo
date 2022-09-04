namespace Web.Data;

public interface IRepository<T> where T : class
{
    IReadOnlyCollection<T> GetAll();
    Task<T?> GetById(string id);
    Task Insert(IEnumerable<T> t);
    Task Remove(T t);
    Task Upsert(IEnumerable<T> t);
    Task Update(IEnumerable<T> t);
}