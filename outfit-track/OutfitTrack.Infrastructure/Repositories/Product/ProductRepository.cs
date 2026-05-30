using Microsoft.EntityFrameworkCore;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class ProductRepository(OutfitTrackContext context) : BaseRepository<Product, InputFilterProduct>(context), IProductRepository
{
    public Product? GetTopByOrders()
    {
        return _context.Product!
            .Include(p => p.ListOrderItem)
            .Where(p => p.ListOrderItem != null && p.ListOrderItem.Count > 0)
            .OrderByDescending(p => p.ListOrderItem!.Count)
            .FirstOrDefault();
    }

    public OutputMetricsProduct GetMetrics()
    {
        var totalProducts = _context.Product!.Count();
        var averagePrice = _context.Product!.Any() ? _context.Product!.Average(p => p.Price ?? 0) : 0;

        return new OutputMetricsProduct
        {
            TotalProducts = totalProducts,
            AveragePrice = Math.Round(averagePrice, 2)
        };
    }

    protected override IQueryable<Product> BuildFilterQuery(InputFilterProduct filter)
    {
        var query = base.BuildFilterQuery(filter);

        if (!string.IsNullOrEmpty(filter.Code))
            query = query.Where(p => p.Code != null && p.Code.Contains(filter.Code));

        if (!string.IsNullOrEmpty(filter.Description))
            query = query.Where(p => p.Description != null && p.Description.Contains(filter.Description));

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(p => p.Category != null && p.Category.Contains(filter.Category));

        if (!string.IsNullOrEmpty(filter.Brand))
            query = query.Where(p => p.Brand != null && p.Brand.Contains(filter.Brand));

        if (filter.OrderBy.HasValue)
        {
            query = filter.OrderByDescending
                ? ApplyOrderByDescending(query, filter.OrderBy.Value)
                : ApplyOrderBy(query, filter.OrderBy.Value);
        }

        return query;
    }

    private static IQueryable<Product> ApplyOrderBy(IQueryable<Product> query, EnumOrderByProduct orderBy)
    {
        return orderBy switch
        {
            EnumOrderByProduct.Code => query.OrderBy(p => p.Code),
            EnumOrderByProduct.Description => query.OrderBy(p => p.Description),
            EnumOrderByProduct.Price => query.OrderBy(p => p.Price),
            EnumOrderByProduct.Brand => query.OrderBy(p => p.Brand),
            EnumOrderByProduct.Category => query.OrderBy(p => p.Category),
            _ => query.OrderBy(p => p.Id)
        };
    }

    private static IQueryable<Product> ApplyOrderByDescending(IQueryable<Product> query, EnumOrderByProduct orderBy)
    {
        return orderBy switch
        {
            EnumOrderByProduct.Code => query.OrderByDescending(p => p.Code),
            EnumOrderByProduct.Description => query.OrderByDescending(p => p.Description),
            EnumOrderByProduct.Price => query.OrderByDescending(p => p.Price),
            EnumOrderByProduct.Brand => query.OrderByDescending(p => p.Brand),
            EnumOrderByProduct.Category => query.OrderByDescending(p => p.Category),
            _ => query.OrderByDescending(p => p.Id)
        };
    }
}