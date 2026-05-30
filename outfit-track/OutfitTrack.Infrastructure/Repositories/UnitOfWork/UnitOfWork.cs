using Microsoft.Extensions.DependencyInjection;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class UnitOfWork(OutfitTrackContext context, IServiceProvider serviceProvider) : IUnitOfWork, IDisposable
{
    private readonly OutfitTrackContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly Dictionary<Type, object> _repositories = [];
    private bool _disposed = false;

    public TIBaseRepository GetRepository<TIBaseRepository, TEntity, TInputFilter>()
        where TIBaseRepository : IBaseRepository<TEntity, TInputFilter>
        where TEntity : BaseEntity<TEntity>
        where TInputFilter : class
    {
        var type = typeof(TIBaseRepository);

        if (_repositories.TryGetValue(type, out var cachedRepository))
            return (TIBaseRepository)cachedRepository;

        var repository = _serviceProvider.GetService<TIBaseRepository>() ??
            throw new InvalidOperationException($"No concrete implementation found for {type.Name}");

        _repositories[type] = repository;
        return repository;
    }

    public void Commit()
    {
        if (!_disposed)
        {
            _context.SaveChanges();
        }
        else
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context?.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
    }
}