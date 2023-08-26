using Web.Models;

namespace Web.Data;

public class SettingRepository : RepositoryBase<Setting>
{
    public SettingRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
}