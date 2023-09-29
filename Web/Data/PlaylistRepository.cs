using Web.Models;

namespace Web.Data;

public class PlaylistRepository : RepositoryBase<Playlist>
{
    public PlaylistRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override Task Upsert(IEnumerable<Playlist> playlists)
    {
        playlists = playlists.ToList();
        IEnumerable<Playlist> playlistsInDb = CustomDbContext.Playlists.ToList();
        IEnumerable<Playlist> toAdd = playlists.ExceptBy(playlistsInDb.Select(x=>x.Id), x => x.Id);
        IEnumerable<Playlist> toDelete = playlistsInDb.ExceptBy(playlists.Select(x=>x.Id), x => x.Id);
        IEnumerable<Playlist> toUpdate = playlistsInDb.IntersectBy(playlists.Select(x=>x.Id), x => x.Id);

        CustomDbContext.Playlists.RemoveRange(toDelete);
        CustomDbContext.Playlists.AddRange(toAdd);
        CustomDbContext.Playlists.UpdateRange(toUpdate);
        return Task.CompletedTask;
    }
}