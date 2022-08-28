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
        return _dbContext.Libraries.Include(x => x.Server).AsNoTracking();
    }

    public async Task<Models.Library> GetById(string id)
    {
        return _dbContext.Libraries.Include(x => x.Server).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public async Task Insert(IEnumerable<Models.Library> t)
    {
        _dbContext.Libraries.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(Models.Library t)
    {
            var toRemove = _dbContext.Libraries.AsNoTracking().FirstOrDefault(x => x.Id == t.Id);
            if (toRemove != null) 
                _dbContext.Libraries.Remove(toRemove);
            await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Models.Library> t)
    {
        foreach (var libraryFromApi in t)
        {
            var libraryToUpdate = _dbContext.Libraries.FirstOrDefault(x => x.Id == libraryFromApi.Id);
            if (libraryToUpdate == null)
                await _dbContext.Libraries.AddAsync(libraryFromApi);
            else
            {
                libraryToUpdate.Key = libraryFromApi.Key;
                libraryToUpdate.Name = libraryFromApi.Name;
                libraryToUpdate.Type = libraryFromApi.Type;
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Models.Library> t)
    {
        _dbContext.Libraries.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }
}