using Web.Models;

namespace Web.Data;

public class SettingRepository : RepositoryBase<Setting>
{
    public SettingRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }
    
}