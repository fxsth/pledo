using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class LibraryRepository : RepositoryBase<Library>
{
    public LibraryRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<Models.Library>> GetAll()
    {
        return DbContext.Libraries.Include(x => x.Server).AsNoTracking();
    }

    public override async Task<Models.Library> GetById(string id)
    {
        return DbContext.Libraries.Include(x => x.Server).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public override async  Task Insert(IEnumerable<Models.Library> t)
    {
        foreach (var library in t)
        {
            DbContext.Entry(library.Server).State = EntityState.Detached;

        }
        await DbContext.AddRangeAsync(t);
    }

    public override async  Task Remove(Models.Library t)
    {
            var toRemove = DbContext.Libraries.AsNoTracking().FirstOrDefault(x => x.Id == t.Id);
            if (toRemove != null) 
                DbContext.Libraries.Remove(toRemove);
    }

    public override async  Task Upsert(IEnumerable<Models.Library> t)
    {
        foreach (var libraryFromApi in t)
        {
            var libraryToUpdate = DbContext.Libraries.FirstOrDefault(x => x.Id == libraryFromApi.Id);
            if (libraryToUpdate == null)
                await DbContext.Libraries.AddAsync(libraryFromApi);
            else
            {
                libraryToUpdate.Key = libraryFromApi.Key;
                libraryToUpdate.Name = libraryFromApi.Name;
                libraryToUpdate.Type = libraryFromApi.Type;
            }
        }

    }

    public override async  Task Update(IEnumerable<Models.Library> t)
    {
        DbContext.Libraries.RemoveRange(t);
    }
}