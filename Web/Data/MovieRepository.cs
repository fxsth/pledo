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
        IEnumerable<Movie> movies = t.ToList();
        var moviesToUpsert = movies.ToHashSet();
        var moviesToDelete = moviesInDb.Except(moviesToUpsert, new MovieEqualityComparer());
        var moviesToInsert = moviesToUpsert.Except(moviesInDb, new MovieEqualityComparer());
        var moviesToUpdate = moviesInDb.Intersect(moviesToUpsert, new MovieEqualityComparer());
        DbContext.Movies.RemoveRange(moviesToDelete);
        DbContext.Movies.AddRange(moviesToInsert);
        DbContext.Movies.UpdateRange(moviesToUpdate);

        Upsert(movies.SelectMany(x => x.MediaFiles));
        
        return Task.CompletedTask;
    }

    private Task Upsert(IEnumerable<MediaFile> t)
    {        
        var inDb = DbContext.MediaFiles.ToHashSet();
        var toUpsert = t.ToHashSet();
        var toDelete = inDb.Except(toUpsert);
        var toInsert = toUpsert.Except(inDb);
        var toUpdate = inDb.Intersect(toUpsert);
        DbContext.MediaFiles.RemoveRange(toDelete);
        DbContext.MediaFiles.AddRange(toInsert);
        DbContext.MediaFiles.UpdateRange(toUpdate);
        return Task.CompletedTask;
    }
}