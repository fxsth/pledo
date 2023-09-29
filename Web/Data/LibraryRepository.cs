using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class LibraryRepository : RepositoryBase<Library>
{
    public LibraryRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override async Task<Models.Library> GetById(string id)
    {
        return CustomDbContext.Libraries.Include(x => x.Server).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public override async  Task Upsert(IEnumerable<Models.Library> t)
    {
        foreach (var libraryFromApi in t)
        {
            var libraryToUpdate = CustomDbContext.Libraries.FirstOrDefault(x => x.Id == libraryFromApi.Id);
            if (libraryToUpdate == null)
                await CustomDbContext.Libraries.AddAsync(libraryFromApi);
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
        CustomDbContext.Libraries.RemoveRange(t);
    }
}