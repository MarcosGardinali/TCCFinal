using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;

namespace OutfitTrack.Domain.Interfaces;

public interface IOrderRepository : IBaseRepository<Order, InputFilterOrder>
{
    long GetNextNumber();
    OutputMetricsOrder GetMetrics(DateTime start, DateTime end);
    OutputSalesMetricsOrder GetSalesMetrics(DateTime start, DateTime end);
}