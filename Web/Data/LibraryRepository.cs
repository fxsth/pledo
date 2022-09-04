using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class LibraryRepository : RepositoryBase<Library>
{
    public LibraryRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Models.Library> GetById(string id)
    {
        return DbContext.Libraries.Include(x => x.Server).AsNoTracking().FirstOrDefault(x => x.Id == id);
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