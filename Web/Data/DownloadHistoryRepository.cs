using Web.Models;

namespace Web.Data;

public class DownloadHistoryRepository : RepositoryBase<DownloadElement>
{
    public DownloadHistoryRepository(DbContext dbContext) : base(dbContext)
    {
    }
}