using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class MovieRepository : RepositoryBase<Movie>
{
    public MovieRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async  Task Insert(IEnumerable<Movie> t)
    {
        DbContext.Movies.AddRange(t);
    }

    public override async  Task Remove(Movie t)
    {
        var toRemove = DbContext.Movies.AsNoTracking().FirstOrDefault(x => x.RatingKey == t.RatingKey);
        if (toRemove != null) 
            DbContext.Movies.Remove(toRemove);
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