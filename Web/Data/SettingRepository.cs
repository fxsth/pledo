using Web.Models;

namespace Web.Data;

public class SettingRepository : RepositoryBase<KeyValueSetting>
{
    public SettingRepository(DbContext dbContext) : base(dbContext)
    {
    }
    
}