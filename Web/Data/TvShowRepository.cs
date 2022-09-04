using Web.Models;

namespace Web.Data;

public class TvShowRepository : RepositoryBase<TvShow>
{
    public TvShowRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
}