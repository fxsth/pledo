using Microsoft.EntityFrameworkCore;
using Web.Extensions;
using Web.Models;

namespace Web.Data;

public class ServerRepository : RepositoryBase<Server>
{
    public ServerRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override async Task<Server> GetById(string id)
    {
        return CustomDbContext.Servers.Include(x => x.Connections).AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public override async  Task Remove(Server t)
    {
        var toRemove = CustomDbContext.Servers.Where(x => x.Id == t.Id).Include(x => x.Connections).FirstOrDefault();
        await CustomDbContext.Libraries.Where(x => x.Server == toRemove).LoadAsync();
        if (toRemove != null)
        {
            CustomDbContext.Remove(toRemove);
        }

    }

    public async  Task Remove(IEnumerable<Server> t)
    {
        CustomDbContext.Servers.RemoveRange(t);
    }

    public override async Task Upsert(IEnumerable<Server> t)
    {
        foreach (var serverFromApi in t)
        {
            var serverToUpdate = CustomDbContext.Servers.Where(x => x.Id == serverFromApi.Id).Include(x => x.Connections)
                .FirstOrDefault();
            if (serverToUpdate == null)
                await CustomDbContext.AddAsync(serverFromApi);
            else
            {
                serverToUpdate.AccessToken = serverFromApi.AccessToken;
                serverToUpdate.LastKnownUri = serverFromApi.LastKnownUri;
                serverToUpdate.LastModified = DateTimeOffset.Now;
                serverToUpdate.IsOnline = serverFromApi.IsOnline;
                CustomDbContext.MergeCollections(serverToUpdate.Connections, serverFromApi.Connections, x => x.Uri);
            }
        
        }
    }
}