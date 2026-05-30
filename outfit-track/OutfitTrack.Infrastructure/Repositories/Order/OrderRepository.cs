using Microsoft.EntityFrameworkCore;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class OrderRepository(OutfitTrackContext context) : BaseRepository<Order, InputFilterOrder>(context), IOrderRepository
{
    public long GetNextNumber()
    {
        var lastOrder = _context.Order!
            .AsNoTracking()
            .OrderByDescending(x => x.Number)
            .FirstOrDefault();

        return (lastOrder?.Number ?? 0) + 1;
    }

    public OutputMetricsOrder GetMetrics(DateTime start, DateTime end)
    {
        var query = _context.Order!
            .AsNoTracking()
            .Where(o => o.CreationDate >= start && o.CreationDate <= end);

        var totalOrders = query.Count();
        var pendingOrdersCount = query.Count(o => o.Status == EnumStatusOrder.Pending);

        return new OutputMetricsOrder
        {
            TotalOrders = totalOrders,
            TotalRevenue = CalculateTotalRevenue(query),
            PendingOrdersCount = pendingOrdersCount,
            OrdersPerDay = GetOrdersPerDay(query),
            AverageTicket = CalculateAverageTicket(query),
            OrdersByStatusCount = GetOrdersByStatusCount(query),
            ConversionRate = CalculateConversionRate(query),
            AverageReturnTime = CalculateAverageReturnTimeDays(query),
            ReturnRate = CalculateReturnRate(query)
        };
    }

    public OutputSalesMetricsOrder GetSalesMetrics(DateTime start, DateTime end)
    {
        var query = _context.Order!
            .AsNoTracking()
            .Where(o => o.CreationDate >= start && o.CreationDate <= end);

        var ordersWithSales = GetOrdersWithSalesInfo(query);

        var totalRevenue = ordersWithSales.Sum(x => x.Revenue);
        var totalSalesCount = ordersWithSales.Count;
        var averageTicket = totalSalesCount > 0 ? Math.Round(totalRevenue / totalSalesCount, 2) : 0;
        var totalItemsSoldCount = ordersWithSales.Sum(x => x.ItemsCount);

        return new OutputSalesMetricsOrder
        {
            TotalRevenue = totalRevenue,
            TotalSalesCount = totalSalesCount,
            AverageTicket = averageTicket,
            TotalItemsSoldCount = totalItemsSoldCount
        };
    }

    #region Private Helper Methods
    private static decimal CalculateConversionRate(IQueryable<Order> query)
    {
        var totalOrders = query.AsNoTracking().Count();
        if (totalOrders == 0)
            return 0;

        var ordersWithSales = query
            .AsNoTracking()
            .Count(o => o.ListOrderItem!.Any(oi => oi.Status == EnumStatusOrderItem.Bought));

        return Math.Round((decimal)ordersWithSales / totalOrders * 100, 2);
    }

    private static decimal CalculateAverageTicket(IQueryable<Order> query)
    {
        var ordersWithSales = GetOrdersWithSalesInfo(query);

        if (ordersWithSales.Count == 0)
            return 0;

        var totalRevenue = ordersWithSales.Sum(x => x.Revenue);
        return Math.Round(totalRevenue / ordersWithSales.Count, 2);
    }

    private static List<OutputDateQuantityOrder> GetOrdersPerDay(IQueryable<Order> query)
    {
        return [.. query
            .AsNoTracking()
            .Where(o => o.CreationDate.HasValue)
            .GroupBy(o => o.CreationDate!.Value.Date)
            .Select(g => new OutputDateQuantityOrder
            {
                Date = g.Key,
                Quantity = g.Count()
            })
            .OrderBy(x => x.Date)];
    }

    private decimal CalculateTotalRevenueThisMonth()
    {
        var now = DateTime.Now;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

        var query = _context.Order!
            .AsNoTracking()
            .Where(o => o.Status == EnumStatusOrder.Closed
                && o.ClosingDate >= firstDayOfMonth
                && o.ClosingDate < firstDayOfNextMonth);

        return CalculateTotalRevenue(query);
    }

    private static List<OutputStatusQuantityOrder> GetOrdersByStatusCount(IQueryable<Order> query)
    {
        return [.. query
            .AsNoTracking()
            .GroupBy(o => o.Status)
            .Select(g => new OutputStatusQuantityOrder
            {
                Status = g.Key.ToString() ?? "Unknown",
                Quantity = g.Count()
            })];
    }

    private static decimal CalculateTotalRevenue(IQueryable<Order> query)
    {
        return query
            .AsNoTracking()
            .SelectMany(o => o.ListOrderItem!)
            .Where(oi => oi.Status == EnumStatusOrderItem.Bought
                && oi.Product != null
                && oi.Product.Price.HasValue)
            .Sum(oi => (decimal?)oi.Product!.Price) ?? 0;
    }

    private static double CalculateAverageReturnTimeDays(IQueryable<Order> query)
    {
        var closedOrders = query
            .AsNoTracking()
            .Where(o => o.Status == EnumStatusOrder.Closed
                && o.CreationDate.HasValue
                && o.ClosingDate.HasValue)
            .Select(o => new
            {
                CreationDate = o.CreationDate!.Value,
                ClosingDate = o.ClosingDate!.Value
            })
            .ToList();

        if (closedOrders.Count == 0)
            return 0;

        var totalDays = closedOrders
            .Sum(o => (o.ClosingDate - o.CreationDate).TotalDays);

        return Math.Round(totalDays / closedOrders.Count, 1);
    }

    private static decimal CalculateReturnRate(IQueryable<Order> query)
    {
        var boughtCount = query
            .AsNoTracking()
            .SelectMany(o => o.ListOrderItem!)
            .Count(oi => oi.Status == EnumStatusOrderItem.Bought);

        var returnedCount = query
            .AsNoTracking()
            .SelectMany(o => o.ListOrderItem!)
            .Count(oi => oi.Status == EnumStatusOrderItem.Returned);

        var totalRelevantItems = boughtCount + returnedCount;

        if (totalRelevantItems == 0)
            return 0;

        return Math.Round((decimal)returnedCount / totalRelevantItems * 100, 2);
    }

    private static List<(decimal Revenue, int ItemsCount)> GetOrdersWithSalesInfo(IQueryable<Order> query)
    {
        var data = query
            .AsNoTracking()
            .Select(o => new
            {
                Revenue = o.ListOrderItem!
                    .Where(oi => oi.Status == EnumStatusOrderItem.Bought
                        && oi.Product != null
                        && oi.Product.Price.HasValue)
                    .Sum(oi => (decimal?)oi.Product!.Price) ?? 0,
                ItemsCount = o.ListOrderItem!
                    .Count(oi => oi.Status == EnumStatusOrderItem.Bought)
            })
            .Where(x => x.Revenue > 0)
            .ToList();

        return data.Select(x => (x.Revenue, x.ItemsCount)).ToList();
    }
    #endregion

    protected override IQueryable<Order> ApplyIncludes(IQueryable<Order> query)
    {
        return query.Include(o => o.Customer)
                   .Include(o => o.ListOrderItem!)
                   .ThenInclude(oi => oi.Product);
    }

    protected override IQueryable<Order> BuildFilterQuery(InputFilterOrder filter)
    {
        var query = base.BuildFilterQuery(filter);

        if (filter.Number.HasValue)
            query = query.Where(o => o.Number == filter.Number.Value);

        if (!string.IsNullOrEmpty(filter.CustomerName))
        {
            query = query.Where(o =>
                (o.Customer!.FirstName != null && o.Customer.FirstName.Contains(filter.CustomerName)) ||
                (o.Customer.LastName != null && o.Customer.LastName.Contains(filter.CustomerName)));
        }

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(o => o.CreationDate!.Value.Date >= filter.DateFrom.Value.Date);

        if (filter.DateTo.HasValue)
            query = query.Where(o => o.CreationDate!.Value.Date <= filter.DateTo.Value.Date);

        if (filter.ProductId.HasValue || !string.IsNullOrEmpty(filter.Variation) || filter.ItemStatus.HasValue)
        {
            query = query.Where(o => o.ListOrderItem != null && o.ListOrderItem.Any(oi =>
                (!filter.ProductId.HasValue || oi.ProductId == filter.ProductId.Value) &&
                (string.IsNullOrEmpty(filter.Variation) || (oi.Variation != null && oi.Variation.Contains(filter.Variation))) &&
                (!filter.ItemStatus.HasValue || oi.Status == filter.ItemStatus.Value)
            ));
        }

        if (filter.OrderBy.HasValue)
        {
            query = filter.OrderByDescending
                ? ApplyOrderByDescending(query, filter.OrderBy.Value)
                : ApplyOrderBy(query, filter.OrderBy.Value);
        }

        return query;
    }

    private static IQueryable<Order> ApplyOrderBy(IQueryable<Order> query, EnumOrderByOrder orderBy)
    {
        return orderBy switch
        {
            EnumOrderByOrder.Number => query.OrderBy(o => o.Number),
            EnumOrderByOrder.CustomerId => query.OrderBy(o => o.CustomerId),
            EnumOrderByOrder.Status => query.OrderBy(o => o.Status),
            EnumOrderByOrder.ClosingDate => query.OrderBy(o => o.ClosingDate),
            EnumOrderByOrder.CreationDate => query.OrderBy(o => o.CreationDate),
            _ => query.OrderBy(o => o.Id)
        };
    }

    private static IQueryable<Order> ApplyOrderByDescending(IQueryable<Order> query, EnumOrderByOrder orderBy)
    {
        return orderBy switch
        {
            EnumOrderByOrder.Number => query.OrderByDescending(o => o.Number),
            EnumOrderByOrder.CustomerId => query.OrderByDescending(o => o.CustomerId),
            EnumOrderByOrder.Status => query.OrderByDescending(o => o.Status),
            EnumOrderByOrder.ClosingDate => query.OrderByDescending(o => o.ClosingDate),
            EnumOrderByOrder.CreationDate => query.OrderByDescending(o => o.CreationDate),
            _ => query.OrderByDescending(o => o.Id)
        };
    }
}