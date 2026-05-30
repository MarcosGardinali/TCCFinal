namespace OutfitTrack.Arguments;

public class OutputMetricsOrder
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrdersCount { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageTicket { get; set; }
    public double AverageReturnTime { get; set; }
    public decimal ReturnRate { get; set; }
    public List<OutputDateQuantityOrder> OrdersPerDay { get; set; } = [];
    public List<OutputStatusQuantityOrder> OrdersByStatusCount { get; set; } = [];
}
