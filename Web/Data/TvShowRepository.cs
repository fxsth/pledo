using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class TvShowRepository : RepositoryBase<TvShow>
{
    public TvShowRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public override IEnumerable<TvShow> Get(
        Expression<Func<TvShow, bool>>? filter = null,
        Func<IQueryable<TvShow>, IOrderedQueryable<TvShow>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TvShow> query = DbContext.Set<TvShow>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        query = query.Include(x => x.Episodes)
            .ThenInclude(e => e.MediaFiles);


        if (orderBy != null)
        {
            return orderBy(query).ToList();
        }
        else
        {
            return query.ToList();
        }
    }
}