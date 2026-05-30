using OutfitTrack.Domain.Entities;

namespace OutfitTrack.Domain.Interfaces;

public interface IUnitOfWork
{
    TIBaseRepository GetRepository<TIBaseRepository, TEntity, TInputFilter>()
        where TIBaseRepository : IBaseRepository<TEntity, TInputFilter>
        where TEntity : BaseEntity<TEntity>
        where TInputFilter : class;

    void Commit();
}