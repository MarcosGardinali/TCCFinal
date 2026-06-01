using System.Collections.Generic;

namespace OutfitTrack.Arguments;

public class OutputDashboard
{
    public OutputMetricsOrder? OrderMetrics { get; set; }
    public OutputSalesMetricsOrder? SalesMetrics { get; set; }
    public OutputMetricsCustomer? CustomerMetrics { get; set; }
    public int TotalProducts { get; set; }
    public List<OutputOrder> ActiveOrders { get; set; } = new List<OutputOrder>();
}
