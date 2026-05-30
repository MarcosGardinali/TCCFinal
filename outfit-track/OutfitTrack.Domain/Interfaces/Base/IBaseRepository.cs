using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using System.Linq.Expressions;

namespace OutfitTrack.Domain.Interfaces;

public interface IBaseRepository<TEntity, TInputFilter>
    where TEntity : BaseEntity<TEntity>
    where TInputFilter : class
{
    PaginatedResult<TEntity>? GetAllByFilter(TInputFilter filter, int pageNumber, int pageSize);
    TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    IEnumerable<TEntity>? GetList(Expression<Func<TEntity, bool>> predicate);
    TEntity? Create(TEntity entity);
    TEntity? Update(TEntity entity);
    bool Delete(TEntity entity);
}

#region All Parameters 
public interface IBaseRepository_0 : IBaseRepository<BaseEntity_0, object> { }
#endregion

#region TInputFilter
public interface IBaseRepository_1<TEntity> : IBaseRepository<TEntity, object>
    where TEntity : BaseEntity<TEntity>
{ }
#endregion