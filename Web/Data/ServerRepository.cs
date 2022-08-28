﻿using Microsoft.EntityFrameworkCore;
using Web.Extensions;
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
        return _dbContext.Servers.Include(x => x.Connections).AsNoTracking().ToList();
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
        var toRemove = _dbContext.Servers.Where(x => x.Id == t.Id).Include(x => x.Connections).FirstOrDefault();
        await _dbContext.Libraries.Where(x => x.Server == toRemove).LoadAsync();
        if (toRemove != null)
        {
            _dbContext.Remove(toRemove);
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
        foreach (var serverFromApi in t)
        {
            var testAllServersInDb = _dbContext.Servers.AsNoTracking().ToList();
            var serverToUpdate = _dbContext.Servers.Where(x => x.Id == serverFromApi.Id).Include(x => x.Connections)
                .FirstOrDefault();
            if (serverToUpdate == null)
                await _dbContext.Servers.AddAsync(serverFromApi);
            else
            {
                serverToUpdate.AccessToken = serverFromApi.AccessToken;
                serverToUpdate.LastKnownUri = serverFromApi.LastKnownUri;
                _dbContext.MergeCollections(serverToUpdate.Connections, serverFromApi.Connections, x => x.Uri);
                // _dbContext.Entry(serverToUpdate).Collection(x=>x.Connections).CurrentValue = null;
                //
                // await _dbContext.SaveChangesAsync();
                //
                // _dbContext.Entry(serverToUpdate).Collection(x=>x.Connections).CurrentValue = serverFromApi.Connections;
                // await _dbContext.SaveChangesAsync();
                // // var toAdd = serverFromApi.Connections.Except(serverToUpdate.Connections);
                // // var toRemove = serverToUpdate.Connections.Except(serverFromApi.Connections);
                // // foreach (var serverConnection in toAdd)
                // // {
                // //     serverToUpdate.Connections.Add(serverConnection);
                // // }
                // // foreach (var serverConnection in toRemove)
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