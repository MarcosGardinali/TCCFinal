namespace OutfitTrack.Arguments;

public class OutputCompleteMetricsOrder
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageTicket { get; set; }
    public double AverageReturnTime { get; set; }
}