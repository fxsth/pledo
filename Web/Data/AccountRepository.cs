using Microsoft.EntityFrameworkCore;

namespace Web.Data;

public class AccountRepository : IAccountRepository
{
    private readonly DbContext _dbContext;

    public AccountRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Models.Account>> GetAll()
    {
        return _dbContext.Accounts.AsNoTracking();
    }

    public async Task<Models.Account> GetById(string id)
    {
        return await _dbContext.Accounts.FindAsync(id);
    }

    public async Task Insert(IEnumerable<Models.Account> t)
    {
        _dbContext.Accounts.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async  Task Remove(IEnumerable<Models.Account> t)
    {
        _dbContext.Accounts.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Models.Account> t)
    {
        foreach (var item in t)
        {
            var toUpdate = _dbContext.Accounts.FirstOrDefault(x => x.Username == item.Username);
            if (toUpdate == null)
                await _dbContext.Accounts.AddAsync(item);
            else
                _dbContext.Accounts.Update(item);
        }
    }

    public async Task Update(IEnumerable<Models.Account> t)
    {
        _dbContext.Accounts.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }
}