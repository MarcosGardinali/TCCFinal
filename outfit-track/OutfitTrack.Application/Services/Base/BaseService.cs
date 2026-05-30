using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Application.Services;

public abstract class BaseService<TIBaseRepository, TInputCreate, TInputUpdate, TEntity, TOutput, TInputFilter>(IUnitOfWork? unitOfWork) : IBaseService<TInputCreate, TInputUpdate, TOutput, TInputFilter>
    where TIBaseRepository : IBaseRepository<TEntity, TInputFilter>
    where TEntity : BaseEntity<TEntity>, new()
    where TInputCreate : class
    where TInputUpdate : class
    where TOutput : class
    where TInputFilter : class
{
    protected Guid _guidApiDataRequest;
    protected readonly IUnitOfWork? _unitOfWork = unitOfWork;
    protected readonly TIBaseRepository? _repository = unitOfWork != null ? unitOfWork!.GetRepository<TIBaseRepository, TEntity, TInputFilter>() : default;

    public void SetGuid(Guid guidApiDataRequest)
    {
        _guidApiDataRequest = guidApiDataRequest;
        GenericModule.SetGuidApiDataRequest(this, guidApiDataRequest);
    }

    #region Read
    public virtual PaginatedResult<TOutput>? GetAllByFilter(TInputFilter filter, int pageNumber, int pageSize)
    {
        var paginatedEntities = _repository!.GetAllByFilter(filter, pageNumber, pageSize);

        if (paginatedEntities?.Items?.Any() == true)
        {
            return new PaginatedResult<TOutput>
            {
                Items = paginatedEntities.Items.Select(FromEntityToOutput),
                TotalItems = paginatedEntities.TotalItems,
                CurrentPage = paginatedEntities.CurrentPage,
                PageSize = paginatedEntities.PageSize
            };
        }
        return null;
    }

    public virtual TOutput? Get(long id)
    {
        var entity = _repository!.Get(x => x.Id == id);
        return entity != null ? FromEntityToOutput(entity) : null;
    }
    #endregion

    #region Create
    public virtual TOutput Create(TInputCreate inputCreate)
    {
        var entity = FromInputCreateToEntity(inputCreate);
        var createdEntity = ExecuteWithCommit(() => _repository!.Create(entity))
            ?? throw new InvalidOperationException("Falha ao criar a entidade.");

        return FromEntityToOutput(createdEntity);
    }
    #endregion

    #region Update
    public virtual TOutput Update(long id, TInputUpdate inputUpdate)
    {
        var oldEntity = _repository!.Get(x => x.Id == id)
            ?? throw new KeyNotFoundException("Id inválido ou inexistente.");

        var updatedEntity = UpdateEntity(oldEntity, inputUpdate);

        var result = ExecuteWithCommit(() => _repository!.Update(oldEntity))
            ?? throw new InvalidOperationException("Falha ao atualizar a entidade.");

        return FromEntityToOutput(result);
    }

    protected static TEntity? UpdateEntity(TEntity oldEntity, TInputUpdate inputUpdate)
    {
        foreach (var property in typeof(TInputUpdate).GetProperties())
        {
            var correspondingProperty = typeof(TEntity).GetProperty(property.Name);
            if (correspondingProperty != null)
            {
                var value = property.GetValue(inputUpdate, null);

                correspondingProperty?.SetValue(oldEntity, value, null);
            }
        }
        return oldEntity;
    }
    #endregion

    #region Delete
    public virtual bool Delete(long id)
    {
        var entity = _repository!.Get(x => x.Id == id)
            ?? throw new KeyNotFoundException("Id inválido ou inexistente.");
        ExecuteWithCommit(() => _repository!.Delete(entity));
        return true;
    }
    #endregion

    protected T ExecuteWithCommit<T>(Func<T> operation)
    {
        var result = operation();
        _unitOfWork!.Commit();
        return result;
    }

    protected void ExecuteWithCommit(Action operation)
    {
        operation();
        _unitOfWork!.Commit();
    }

    #region Mapper
    public virtual TOutput FromEntityToOutput(TEntity entity)
    {
        throw new NotImplementedException("Este método deve ser implementado nas classes derivadas.");
    }

    public virtual TEntity FromInputCreateToEntity(TInputCreate inputCreate)
    {
        throw new NotImplementedException("Este método deve ser implementado nas classes derivadas.");
    }
    #endregion
}

#region All Parameters
public abstract class BaseService_0() : BaseService<IBaseRepository_0, object, object, BaseEntity_0, object, object>(default) { }
#endregion

#region TInputFilter
public abstract class BaseService_1<TIBaseRepository, TInputCreate, TInputUpdate, TEntity, TOutput>(IUnitOfWork? unitOfWork) : BaseService<TIBaseRepository, TInputCreate, TInputUpdate, TEntity, TOutput, object>(unitOfWork)
    where TIBaseRepository : IBaseRepository_1<TEntity>
    where TEntity : BaseEntity<TEntity>, new()
    where TInputCreate : class
    where TInputUpdate : class
    where TOutput : class
{ }
#endregion