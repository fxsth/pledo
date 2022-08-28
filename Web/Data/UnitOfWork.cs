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

    public AccountRepository AccountRepository
    {
        get
        {
            if (_accountRepository == null)
            {
                _accountRepository = new AccountRepository(_dbContext);
            }

            return _accountRepository;
        }
    }
    public ServerRepository ServerRepository
    {
        get
        {
            if (_serverRepository == null)
            {
                _serverRepository = new ServerRepository(_dbContext);
            }

            return _serverRepository;
        }
    }
    public LibraryRepository LibraryRepository
    {
        get
        {
            if (_libraryRepository == null)
            {
                _libraryRepository = new LibraryRepository(_dbContext);
            }

            return _libraryRepository;
        }
    }
    public MovieRepository MovieRepository
    {
        get
        {
            if (_movieRepository == null)
            {
                this._movieRepository = new MovieRepository(_dbContext);
            }

            return _movieRepository;
        }
    }

    public async Task Save()
    {
        await _dbContext.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}