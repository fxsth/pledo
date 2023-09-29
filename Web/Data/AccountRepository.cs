using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class AccountRepository : RepositoryBase<Account>
{
    public AccountRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override async Task<Models.Account> GetById(string id)
    {
        return await CustomDbContext.Accounts.FindAsync(id);
    }

    public override async Task Remove(Models.Account t)
    {
        var toRemove = CustomDbContext.Accounts.AsNoTracking().FirstOrDefault(x => x.Username == t.Username);
        if (toRemove != null) 
            CustomDbContext.Accounts.Remove(toRemove);
    }

    public override async Task Upsert(IEnumerable<Models.Account> t)
    {
        foreach (var item in t)
        {
            var toUpdate = CustomDbContext.Accounts.FirstOrDefault(x => x.Username == item.Username);
            if (toUpdate == null)
                await CustomDbContext.Accounts.AddAsync(item);
            else
                CustomDbContext.Accounts.Update(item);
        }
    }

    public override async Task Update(IEnumerable<Models.Account> t)
    {
        CustomDbContext.Accounts.RemoveRange(t);
    }
}