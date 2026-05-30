using OutfitTrack.Arguments;

namespace OutfitTrack.Application.Interfaces;

public interface IOrderService : IBaseService<InputCreateOrder, InputUpdateOrder, OutputOrder, InputFilterOrder>
{
    bool Close(long id);
    OutputOrder? GetByNumber(long number);
    OutputMetricsOrder GetMetrics(DateTime start, DateTime end);
    OutputSalesMetricsOrder GetSalesMetrics(DateTime start, DateTime end);
}