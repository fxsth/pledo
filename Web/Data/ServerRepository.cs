using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        return _dbContext.Servers.Include(x=>x.Connections).AsNoTracking().ToList();
    }

    public async Task<Server> GetById(string id)
    {
        return _dbContext.Servers.Include(x => x.Connections).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public async Task Insert(IEnumerable<Server> t)
    {
        _dbContext.Servers.AddRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(Server t)
    {
        var toRemove = _dbContext.Servers.FirstOrDefault(x => x.Id == x.Id);
        if (toRemove != null)
        {
            _dbContext.Servers.Remove(toRemove);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(IEnumerable<Server> t)
    {
        _dbContext.Servers.RemoveRange(t);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Upsert(IEnumerable<Server> t)
    {
        var serversInDb = _dbContext.Servers;
        foreach (var serverFromApi in t)
        {
            var serverToUpdate = serversInDb.Include(x=>x.Connections).FirstOrDefault(x => x.Id == serverFromApi.Id);
            if (serverToUpdate == null)
                await _dbContext.Servers.AddAsync(serverFromApi);
            else
            {
                // serverToUpdate.AccessToken = serverFromApi.AccessToken;
                // serverToUpdate.LastKnownUri = null;
                _dbContext.Entry(serverToUpdate).CurrentValues.SetValues(serverFromApi);
                // var toAdd = serverFromApi.Connections.Except(serverToUpdate.Connections);
                // var toRemove = serverToUpdate.Connections.Except(serverFromApi.Connections);
                // foreach (var serverConnection in toAdd)
                // {
                //     serverToUpdate.Connections.Add(serverConnection);
                // }
                // foreach (var serverConnection in toRemove)
                // {
                //     serverToUpdate.Connections.Remove(serverConnection);
                // }
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