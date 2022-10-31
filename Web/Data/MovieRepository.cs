using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class MovieRepository : RepositoryBase<Movie>
{
    public MovieRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override Task Upsert(IEnumerable<Movie> t)
    {        
        var moviesInDb = DbContext.Movies.ToHashSet();
        var moviesToUpsert = t.ToHashSet();
        var moviesToDelete = moviesInDb.Except(moviesToUpsert, new MovieEqualityComparer());
        var moviesToInsert = moviesToUpsert.Except(moviesInDb, new MovieEqualityComparer());
        var moviesToUpdate = moviesInDb.Intersect(moviesToUpsert, new MovieEqualityComparer());
        DbContext.Movies.RemoveRange(moviesToDelete);
        DbContext.Movies.AddRange(moviesToInsert);
        DbContext.Movies.UpdateRange(moviesToUpdate);
        return Task.CompletedTask;
    }
}