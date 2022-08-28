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

    public AccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_dbContext);
    public ServerRepository ServerRepository => _serverRepository ??= new ServerRepository(_dbContext);
    public LibraryRepository LibraryRepository => _libraryRepository ??= new LibraryRepository(_dbContext);
    public MovieRepository MovieRepository => _movieRepository ??= new MovieRepository(_dbContext);

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }

    private bool _disposed = false;

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