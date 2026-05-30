using OutfitTrack.Arguments;

namespace OutfitTrack.Application.Interfaces;

public interface IBaseService<TInputCreate, TInputUpdate, TOutput, TInputFilter>
   where TInputCreate : class
   where TInputUpdate : class
   where TOutput : class
   where TInputFilter : class
{
    PaginatedResult<TOutput>? GetAllByFilter(TInputFilter filter, int pageNumber, int pageSize);
    TOutput? Get(long id);
    TOutput? Create(TInputCreate inputCreate);
    TOutput? Update(long id, TInputUpdate inputUpdate);
    bool Delete(long id);
}

#region AllParameters
public interface IBaseService_0 : IBaseService<object, object, object, object> { }
#endregion

#region AllParameters
public interface IBaseService_1<TInputCreate, TInputUpdate, TOutput> : IBaseService<TInputCreate, TInputUpdate, TOutput, object>
   where TInputCreate : class
   where TInputUpdate : class
   where TOutput : class
{ }
#endregion