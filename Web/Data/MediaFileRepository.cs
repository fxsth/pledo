using Web.Models;
using Web.Models.Helper;

namespace Web.Data;

public class MediaFileRepository : RepositoryBase<MediaFile>
{
    public MediaFileRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }
}