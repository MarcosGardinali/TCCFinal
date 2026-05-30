using OutfitTrack.Arguments;

namespace OutfitTrack.Application.Interfaces;

public interface IProductService : IBaseService<InputCreateProduct, InputUpdateProduct, OutputProduct, InputFilterProduct>
{
    OutputProduct? GetByCode(string code);
    OutputProduct? GetTopByOrders();
    OutputMetricsProduct GetMetrics();
}