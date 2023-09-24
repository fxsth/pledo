using Web.Models;

namespace Web.Data;

public class EpisodeRepository : RepositoryBase<Episode>
{
    public EpisodeRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
    public override Task Upsert(IEnumerable<Episode> t)
    {        
        var episodesInDb = DbContext.Episodes.ToHashSet();
        IEnumerable<Episode> episodes = t.ToList();
        var episodesToUpsert = episodes.ToHashSet();
        var episodesToDelete = episodesInDb.ExceptBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToInsert = episodesToUpsert.ExceptBy(episodesInDb.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToUpdate = episodesInDb.IntersectBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        DbContext.Episodes.RemoveRange(episodesToDelete);
        DbContext.Episodes.AddRange(episodesToInsert);
        DbContext.Episodes.UpdateRange(episodesToUpdate);
        
        Upsert(episodes.SelectMany(x => x.MediaFiles));
        
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