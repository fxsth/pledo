using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class MovieRepository : RepositoryBase<Movie>, IMovieRepository
{
    
    public MovieRepository(DbContext dbContext) : base(dbContext)
    {
    }
    public override async  Task<IEnumerable<Models.Movie>> GetAll()
    {
        return DbContext.Movies.AsNoTracking().ToList();
    }

    public override async Task<Models.Movie> GetById(string id)
    {
        return await DbContext.Movies.FindAsync(id);
    }

    public override async  Task Insert(IEnumerable<Models.Movie> t)
    {
        DbContext.Movies.AddRange(t);
    }

    public override async  Task Remove(Models.Movie t)
    {
        var toRemove = DbContext.Movies.AsNoTracking().FirstOrDefault(x => x.RatingKey == t.RatingKey);
        if (toRemove != null) 
            DbContext.Movies.Remove(toRemove);
    }

    public override async  Task Upsert(IEnumerable<Models.Movie> t)
    {
        var moviesInDb = DbContext.Movies.AsNoTracking().ToHashSet();
        var moviesToUpsert = t.ToHashSet();
        var moviesToDelete = moviesInDb.Except(moviesToUpsert, new MovieEqualityComparer());
        var moviesToInsert = moviesToUpsert.Except(moviesInDb, new MovieEqualityComparer());
        var moviesToUpdate = moviesInDb.Intersect(moviesToUpsert, new MovieEqualityComparer());
        DbContext.Movies.RemoveRange(moviesToDelete);
        DbContext.Movies.AddRange(moviesToInsert);
        DbContext.Movies.UpdateRange(moviesToUpdate);
    }

    public override async Task Update(IEnumerable<Models.Movie> t)
    {
        DbContext.Movies.RemoveRange(t);
    }
}