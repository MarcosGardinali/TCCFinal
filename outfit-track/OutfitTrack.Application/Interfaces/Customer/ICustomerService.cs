using OutfitTrack.Arguments;

namespace OutfitTrack.Application.Interfaces;

public interface ICustomerService : IBaseService<InputCreateCustomer, InputUpdateCustomer, OutputCustomer, InputFilterCustomer>
{
    OutputCustomer? GetByCpf(string cpf);
    OutputCustomer? GetTopByOrders();
    OutputMetricsCustomer GetMetrics();
}