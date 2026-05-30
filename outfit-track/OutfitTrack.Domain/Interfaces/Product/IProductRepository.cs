using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;

namespace OutfitTrack.Domain.Interfaces;

public interface IProductRepository : IBaseRepository<Product, InputFilterProduct>
{
    Product? GetTopByOrders();
    OutputMetricsProduct GetMetrics();
}