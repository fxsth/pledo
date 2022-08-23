using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class ServerRepository : IServerRepository
{
    private readonly DbContext _dbContext;

    public ServerRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Server>> GetAll()
    {
        return _dbContext.Servers.AsNoTracking().ToList();
    }

    public async Task<Server> GetById(string id)
    {
        return _dbContext.Servers.Include(x=>x.Connections).FirstOrDefault(x=>x.Id == id);

    }

    public async Task Insert(IEnumerable<Server> t)
    {
        _dbContext.Servers.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(IEnumerable<Server> t)
    {
        _dbContext.Servers.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Server> t)
    {
        foreach (var server in t)
        {
            var serverToUpdate = _dbContext.Servers.Include(x => x.Connections).FirstOrDefault(x => x.Id == server.Id);
            if (serverToUpdate == null)
                await _dbContext.Servers.AddAsync(server);
            else
            {
                serverToUpdate.Name = server.Name;
                serverToUpdate.Connections = null;
                serverToUpdate.Connections = server.Connections;
                serverToUpdate.AccessToken = server.AccessToken;
                serverToUpdate.LastModified = DateTimeOffset.Now;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task Update(IEnumerable<Server> t)
    {
        _dbContext.Servers.UpdateRange(t);
        await _dbContext.SaveChangesAsync();
    }
}