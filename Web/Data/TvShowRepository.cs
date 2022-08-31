using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class TvShowRepository : RepositoryBase<TvShow>
{
    public TvShowRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public Task Upsert(IEnumerable<TvShow> t)
    {        
        var itemsInDb = DbContext.TvShows.ToHashSet();
        // var itemsToUpsert = t.ToHashSet();
        // var itemsToDelete = itemsInDb.Except(itemsToUpsert);
        // var itemsToInsert = itemsToUpsert.Except(itemsInDb);
        // var itemsToUpdate = itemsInDb.Intersect(itemsToUpsert);
        DbContext.TvShows.RemoveRange(itemsToDelete);
        DbContext.TvShows.AddRange(itemsToInsert);
        DbContext.TvShows.UpdateRange(itemsToUpdate);
        return Task.CompletedTask;
    }
}