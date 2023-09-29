using Web.Models;

namespace Web.Data;

public class DownloadRepository : RepositoryBase<DownloadElement>
{
    public DownloadRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }
}