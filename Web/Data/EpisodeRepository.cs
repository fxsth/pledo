using Web.Models;

namespace Web.Data;

public class EpisodeRepository : RepositoryBase<Episode>
{
    public EpisodeRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
    public Task Upsert(IEnumerable<Episode> t)
    {        
        var episodesInDb = DbContext.Episodes.ToHashSet();
        var episodesToUpsert = t.ToHashSet();
        var episodesToDelete = episodesInDb.ExceptBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToInsert = episodesToUpsert.ExceptBy(episodesInDb.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToUpdate = episodesInDb.IntersectBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        DbContext.Episodes.RemoveRange(episodesToDelete);
        DbContext.Episodes.AddRange(episodesToInsert);
        DbContext.Episodes.UpdateRange(episodesToUpdate);
        return Task.CompletedTask;
    }
}