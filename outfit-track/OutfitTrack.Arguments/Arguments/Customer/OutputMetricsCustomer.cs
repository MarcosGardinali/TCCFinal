namespace OutfitTrack.Arguments;

public class OutputMetricsCustomer
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomersCount { get; set; }
    public int AverageAge { get; set; }
    public List<OutputMonthQuantityOrder> NewCustomersPerMonth { get; set; } = [];
}
