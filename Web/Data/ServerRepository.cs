using Microsoft.EntityFrameworkCore;
using Web.Extensions;
using Web.Models;

namespace Web.Data;

public class ServerRepository : RepositoryBase<Server>
{
    public ServerRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override async  Task<Server> GetById(string id)
    {
        return DbContext.Servers.Include(x => x.Connections).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public override async  Task Insert(IEnumerable<Server> t)
    {
        DbContext.Servers.AddRange(t);
    }

    public override async  Task Remove(Server t)
    {
        var toRemove = DbContext.Servers.Where(x => x.Id == t.Id).Include(x => x.Connections).FirstOrDefault();
        await DbContext.Libraries.Where(x => x.Server == toRemove).LoadAsync();
        if (toRemove != null)
        {
            DbContext.Remove(toRemove);
        }

    }

    public async  Task Remove(IEnumerable<Server> t)
    {
        DbContext.Servers.RemoveRange(t);
    }

    public override async Task Upsert(IEnumerable<Server> t)
    {
        foreach (var serverFromApi in t)
        {
            var serverToUpdate = DbContext.Servers.Where(x => x.Id == serverFromApi.Id).Include(x => x.Connections)
                .FirstOrDefault();
            if (serverToUpdate == null)
                await DbContext.AddAsync(serverFromApi);
            else
            {
                serverToUpdate.AccessToken = serverFromApi.AccessToken;
                serverToUpdate.LastKnownUri = serverFromApi.LastKnownUri;
                DbContext.MergeCollections(serverToUpdate.Connections, serverFromApi.Connections, x => x.Uri);
            }
    
        }
    }
}