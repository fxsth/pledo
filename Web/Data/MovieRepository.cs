using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class MovieRepository : RepositoryBase<Movie>
{
    public MovieRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override Task Upsert(IEnumerable<Movie> t)
    {        
        var moviesInDb = CustomDbContext.Movies.ToHashSet();
        List<Movie> movies = t.ToList();
        var moviesToUpsert = movies.ToHashSet();
        var moviesToDelete = moviesInDb.Except(moviesToUpsert, new MovieEqualityComparer());
        var moviesToInsert = moviesToUpsert.Except(moviesInDb, new MovieEqualityComparer());
        var moviesToUpdate = moviesInDb.Intersect(moviesToUpsert, new MovieEqualityComparer());
        CustomDbContext.Movies.RemoveRange(moviesToDelete);
        CustomDbContext.Movies.AddRange(moviesToInsert);
        CustomDbContext.Movies.UpdateRange(moviesToUpdate);
        return Task.CompletedTask;
    }
    
}