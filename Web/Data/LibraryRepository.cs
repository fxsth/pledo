using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class LibraryRepository : ILibraryRepository
{
    private readonly DbContext _dbContext;

    public LibraryRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Models.Library>> GetAll()
    {
        return _dbContext.Libraries.AsNoTracking();
    }

    public async Task<Models.Library> GetById(string id)
    {
        return _dbContext.Libraries.Include(x=>x.Server).AsNoTracking().FirstOrDefault(x=>x.Id == id);
    }

    public async Task Insert(IEnumerable<Models.Library> t)
    {
        _dbContext.Libraries.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async  Task Remove(IEnumerable<Models.Library> t)
    {
        _dbContext.Libraries.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Models.Library> t)
    {
        foreach (var item in t)
        {
            var toUpdate = _dbContext.Libraries.Include(x => x.Server).FirstOrDefault(x => x.Id == item.Id);
            if (toUpdate == null)
                await _dbContext.Libraries.AddAsync(item);
            else
                _dbContext.Libraries.Update(item);
        }
    }

    public async Task Update(IEnumerable<Models.Library> t)
    {
        _dbContext.Libraries.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }
}