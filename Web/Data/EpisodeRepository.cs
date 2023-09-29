using Web.Models;

namespace Web.Data;

public class EpisodeRepository : RepositoryBase<Episode>
{
    public EpisodeRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }
    
    public override Task Upsert(IEnumerable<Episode> t)
    {        
        var episodesInDb = CustomDbContext.Episodes.ToHashSet();
        IEnumerable<Episode> episodes = t.ToList();
        var episodesToUpsert = episodes.ToHashSet();
        var episodesToDelete = episodesInDb.ExceptBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToInsert = episodesToUpsert.ExceptBy(episodesInDb.Select(x=>x.RatingKey), x=>x.RatingKey);
        var episodesToUpdate = episodesInDb.IntersectBy(episodesToUpsert.Select(x=>x.RatingKey), x=>x.RatingKey);
        CustomDbContext.Episodes.RemoveRange(episodesToDelete);
        CustomDbContext.Episodes.AddRange(episodesToInsert);
        CustomDbContext.Episodes.UpdateRange(episodesToUpdate);
        
        Upsert(episodes.SelectMany(x => x.MediaFiles));
        
        return Task.CompletedTask;
    }
    
    private Task Upsert(IEnumerable<MediaFile> t)
    {        
        var inDb = CustomDbContext.MediaFiles.ToHashSet();
        var toUpsert = t.ToHashSet();
        var toDelete = inDb.Except(toUpsert);
        var toInsert = toUpsert.Except(inDb);
        var toUpdate = inDb.Intersect(toUpsert);
        CustomDbContext.MediaFiles.RemoveRange(toDelete);
        CustomDbContext.MediaFiles.AddRange(toInsert);
        CustomDbContext.MediaFiles.UpdateRange(toUpdate);
        
        return Task.CompletedTask;
    }
}