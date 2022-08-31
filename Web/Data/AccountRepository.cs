using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class AccountRepository : RepositoryBase<Account>
{
    public AccountRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<IEnumerable<Account>> GetAll()
    {
        return DbContext.Accounts.AsNoTracking().ToList();
    }

    public override async Task<Models.Account> GetById(string id)
    {
        return await DbContext.Accounts.FindAsync(id);
    }

    public override async Task Insert(IEnumerable<Models.Account> t)
    {
        DbContext.Accounts.AddRange(t);
    }

    public override async Task Remove(Models.Account t)
    {
        var toRemove = DbContext.Accounts.AsNoTracking().FirstOrDefault(x => x.Username == t.Username);
        if (toRemove != null) 
            DbContext.Accounts.Remove(toRemove);
    }

    public override async Task Upsert(IEnumerable<Models.Account> t)
    {
        foreach (var item in t)
        {
            var toUpdate = DbContext.Accounts.FirstOrDefault(x => x.Username == item.Username);
            if (toUpdate == null)
                await DbContext.Accounts.AddAsync(item);
            else
                DbContext.Accounts.Update(item);
        }
    }

    public override async Task Update(IEnumerable<Models.Account> t)
    {
        DbContext.Accounts.RemoveRange(t);
    }
}