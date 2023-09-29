namespace Web.Data;

public class UnitOfWork : IDisposable
{
    private readonly CustomDbContext _customDbContext;

    public UnitOfWork(CustomDbContext customDbContext)
    {
        _customDbContext = customDbContext;
    }

    private AccountRepository? _accountRepository;
    private ServerRepository? _serverRepository;
    private LibraryRepository? _libraryRepository;
    private MovieRepository? _movieRepository;
    private TvShowRepository? _tvShowRepository;
    private EpisodeRepository? _episodeRepository;
    private SettingRepository? _settingRepository;
    private DownloadRepository? _downloadHistoryRepository;
    private PlaylistRepository? _playlistRepository;
    private MediaFileRepository? _mediaFileRepository;


    public AccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_customDbContext);
    public ServerRepository ServerRepository => _serverRepository ??= new ServerRepository(_customDbContext);
    public LibraryRepository LibraryRepository => _libraryRepository ??= new LibraryRepository(_customDbContext);
    public MovieRepository MovieRepository => _movieRepository ??= new MovieRepository(_customDbContext);
    public MediaFileRepository MediaFileRepository => _mediaFileRepository ??= new MediaFileRepository(_customDbContext);
    public TvShowRepository TvShowRepository => _tvShowRepository ??= new TvShowRepository(_customDbContext);
    public EpisodeRepository EpisodeRepository => _episodeRepository ??= new EpisodeRepository(_customDbContext);
    public SettingRepository SettingRepository => _settingRepository ??= new SettingRepository(_customDbContext);
    public DownloadRepository DownloadRepository =>
        _downloadHistoryRepository ??= new DownloadRepository(_customDbContext);
    public PlaylistRepository PlaylistRepository => _playlistRepository ??= new PlaylistRepository(_customDbContext);

    public async Task Save()
    {
        await _customDbContext.SaveChangesAsync();
    }

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _customDbContext.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}