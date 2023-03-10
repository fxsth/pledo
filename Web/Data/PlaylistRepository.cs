using Web.Models;

namespace Web.Data;

public class PlaylistRepository : RepositoryBase<Playlist>
{
    public PlaylistRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override Task Upsert(IEnumerable<Playlist> playlists)
    {
        playlists = playlists.ToList();
        IEnumerable<Playlist> playlistsInDb = DbContext.Playlists.ToList();
        IEnumerable<Playlist> toAdd = playlists.ExceptBy(playlistsInDb.Select(x=>x.Id), x => x.Id);
        IEnumerable<Playlist> toDelete = playlistsInDb.ExceptBy(playlists.Select(x=>x.Id), x => x.Id);
        IEnumerable<Playlist> toUpdate = playlistsInDb.IntersectBy(playlists.Select(x=>x.Id), x => x.Id);

        DbContext.Playlists.RemoveRange(toDelete);
        DbContext.Playlists.AddRange(toAdd);
        DbContext.Playlists.UpdateRange(toUpdate);
        return Task.CompletedTask;
    }
}