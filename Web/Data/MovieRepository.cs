using Microsoft.EntityFrameworkCore;
using Web.Models.Helper;

namespace Web.Data;

public class MovieRepository : IMovieRepository
{
    private readonly DbContext _dbContext;

    public MovieRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Models.Movie>> GetAll()
    {
        return _dbContext.Movies.AsNoTracking().ToList();
    }

    public async Task<Models.Movie> GetById(string id)
    {
        return await _dbContext.Movies.FindAsync(id);
    }

    public async Task Insert(IEnumerable<Models.Movie> t)
    {
        _dbContext.Movies.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(Models.Movie t)
    {
        var toRemove = _dbContext.Movies.AsNoTracking().FirstOrDefault(x => x.RatingKey == t.RatingKey);
        if (toRemove != null) 
            _dbContext.Movies.Remove(toRemove);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Models.Movie> t)
    {
        var moviesInDb = _dbContext.Movies.AsNoTracking().ToHashSet();
        var moviesToUpsert = t.ToHashSet();
        var moviesToDelete = moviesInDb.Except(moviesToUpsert, new MovieEqualityComparer());
        var moviesToInsert = moviesToUpsert.Except(moviesInDb, new MovieEqualityComparer());
        var moviesToUpdate = moviesInDb.Intersect(moviesToUpsert, new MovieEqualityComparer());
        _dbContext.Movies.RemoveRange(moviesToDelete);
        _dbContext.Movies.AddRange(moviesToInsert);
        _dbContext.Movies.UpdateRange(moviesToUpdate);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Models.Movie> t)
    {
        _dbContext.Movies.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }
}