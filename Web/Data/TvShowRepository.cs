using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class TvShowRepository : RepositoryBase<TvShow>
{
    public TvShowRepository(CustomDbContext customDbContext) : base(customDbContext)
    {
    }

    public override async Task<IEnumerable<TvShow>> Get(Expression<Func<TvShow, bool>>? filter = null,
        Func<IQueryable<TvShow>, IOrderedQueryable<TvShow>>? orderBy = null,
        string includeProperties = "", int offset = 0, int size = 0)
    {
        IQueryable<TvShow> query = CustomDbContext.Set<TvShow>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        query = query.Include(x => x.Episodes)
            .ThenInclude(e => e.MediaFiles);


        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }
}