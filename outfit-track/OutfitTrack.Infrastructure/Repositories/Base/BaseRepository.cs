using Microsoft.EntityFrameworkCore;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;
using System.Linq.Expressions;

namespace OutfitTrack.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity, TInputFilter>(OutfitTrackContext context) : IBaseRepository<TEntity, TInputFilter>
    where TEntity : BaseEntity<TEntity>, new()
    where TInputFilter : class
{
    protected readonly OutfitTrackContext _context = context;

    #region Read
    public virtual PaginatedResult<TEntity>? GetAllByFilter(TInputFilter filter, int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, Math.Min(100, pageSize));

        IQueryable<TEntity> query = BuildFilterQuery(filter);
        var totalItems = query.Count();

        if (totalItems == 0)
            return new PaginatedResult<TEntity> { Items = [], TotalItems = 0, CurrentPage = pageNumber, PageSize = pageSize };

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<TEntity>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };
    }

    public virtual TEntity? Get(Expression<Func<TEntity, bool>> predicate)
    {
        var query = _context.Set<TEntity>().AsNoTracking();
        query = ApplyIncludes(query);
        return query.FirstOrDefault(predicate);
    }

    public virtual IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
    {
        return [.. _context.Set<TEntity>().AsNoTracking().Where(predicate)];
    }

    protected virtual IQueryable<TEntity> BuildFilterQuery(TInputFilter filter)
    {
        var query = _context.Set<TEntity>().AsNoTracking();
        return ApplyIncludes(query);
    }

    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
    {
        return query;
    }
    #endregion

    #region Create
    public virtual TEntity? Create(TEntity entity)
    {
        var trackedEntity = _context.Set<TEntity>().Add(entity.SetCreateData()).Entity;
        return trackedEntity;
    }

    public virtual TEntity? Update(TEntity entity)
    {
        var trackedEntity = _context.Set<TEntity>().Update(entity.SetUpdateData()).Entity;
        return trackedEntity;
    }

    public virtual bool Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return true;
    }
    #endregion

    protected int GetTotalCount() => _context.Set<TEntity>().Count();
}

#region TInputFilter
public class BaseRepository_1<TEntity>(OutfitTrackContext context) : BaseRepository<TEntity, object>(context)
    where TEntity : BaseEntity<TEntity>, new()
{ }
#endregion