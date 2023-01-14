namespace Web.Data;

public class UnitOfWork : IDisposable
{
    private readonly DbContext _dbContext;

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private AccountRepository? _accountRepository;
    private ServerRepository? _serverRepository;
    private LibraryRepository? _libraryRepository;
    private MovieRepository? _movieRepository;
    private TvShowRepository? _tvShowRepository;
    private EpisodeRepository? _episodeRepository;
    private SettingRepository? _settingRepository;
    private DownloadRepository? _downloadHistoryRepository;

    public AccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_dbContext);
    public ServerRepository ServerRepository => _serverRepository ??= new ServerRepository(_dbContext);
    public LibraryRepository LibraryRepository => _libraryRepository ??= new LibraryRepository(_dbContext);
    public MovieRepository MovieRepository => _movieRepository ??= new MovieRepository(_dbContext);
    public TvShowRepository TvShowRepository => _tvShowRepository ??= new TvShowRepository(_dbContext);
    public EpisodeRepository EpisodeRepository => _episodeRepository ??= new EpisodeRepository(_dbContext);
    public SettingRepository SettingRepository => _settingRepository ??= new SettingRepository(_dbContext);
    public DownloadRepository DownloadRepository =>
        _downloadHistoryRepository ??= new DownloadRepository(_dbContext);

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
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