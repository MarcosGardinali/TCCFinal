using Microsoft.EntityFrameworkCore;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class CustomerRepository(OutfitTrackContext context) : BaseRepository<Customer, InputFilterCustomer>(context), ICustomerRepository
{
    public Customer? GetTopByOrders()
    {
        return _context.Customer!
            .Include(c => c.ListOrder)
            .Where(c => c.ListOrder != null && c.ListOrder.Count > 0)
            .OrderByDescending(c => c.ListOrder!.Count)
            .FirstOrDefault();
    }

    public OutputMetricsCustomer GetMetrics()
    {
        var totalCustomers = _context.Customer!.Count();
        var activeCustomersCount = _context.Customer!
            .Include(c => c.ListOrder)
            .Count(c => c.ListOrder!.Any());

        var customersWithBirthDate = _context.Customer!
            .Where(c => c.BirthDate.HasValue)
            .Select(c => c.BirthDate!.Value)
            .ToList();

        var averageAge = customersWithBirthDate.Count != 0
            ? customersWithBirthDate.Average(b => (DateTime.Today - b).Days / 365.25)
            : 0;

        var newCustomersPerMonth = _context.Customer!
            .Where(c => c.CreationDate.HasValue)
            .GroupBy(c => new { c.CreationDate!.Value.Year, c.CreationDate!.Value.Month })
            .Select(g => new OutputMonthQuantityOrder
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Quantity = g.Count()
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToList();

        return new OutputMetricsCustomer
        {
            TotalCustomers = totalCustomers,
            ActiveCustomersCount = activeCustomersCount,
            AverageAge = (int)Math.Round(averageAge),
            NewCustomersPerMonth = newCustomersPerMonth
        };
    }

    protected override IQueryable<Customer> BuildFilterQuery(InputFilterCustomer filter)
    {
        var query = base.BuildFilterQuery(filter);

        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(c =>
                (c.FirstName != null && c.FirstName.Contains(filter.Name)) ||
                (c.LastName != null && c.LastName.Contains(filter.Name)));
        }

        if (!string.IsNullOrEmpty(filter.Cpf))
            query = query.Where(c => c.Cpf != null && c.Cpf.Contains(filter.Cpf));

        if (!string.IsNullOrEmpty(filter.Email))
            query = query.Where(c => c.Email != null && c.Email.Contains(filter.Email));

        if (!string.IsNullOrEmpty(filter.MobilePhoneNumber))
            query = query.Where(c => c.MobilePhoneNumber != null && c.MobilePhoneNumber.Contains(filter.MobilePhoneNumber));

        if (filter.OrderBy.HasValue)
        {
            query = filter.OrderByDescending
                ? ApplyOrderByDescending(query, filter.OrderBy.Value)
                : ApplyOrderBy(query, filter.OrderBy.Value);
        }

        return query;
    }

    private static IQueryable<Customer> ApplyOrderBy(IQueryable<Customer> query, EnumOrderByCustomer orderBy)
    {
        return orderBy switch
        {
            EnumOrderByCustomer.FirstName => query.OrderBy(c => c.FirstName),
            EnumOrderByCustomer.LastName => query.OrderBy(c => c.LastName),
            EnumOrderByCustomer.Cpf => query.OrderBy(c => c.Cpf),
            EnumOrderByCustomer.Email => query.OrderBy(c => c.Email),
            EnumOrderByCustomer.CreationDate => query.OrderBy(c => c.CreationDate),
            _ => query.OrderBy(c => c.Id)
        };
    }

    private static IQueryable<Customer> ApplyOrderByDescending(IQueryable<Customer> query, EnumOrderByCustomer orderBy)
    {
        return orderBy switch
        {
            EnumOrderByCustomer.FirstName => query.OrderByDescending(c => c.FirstName),
            EnumOrderByCustomer.LastName => query.OrderByDescending(c => c.LastName),
            EnumOrderByCustomer.Cpf => query.OrderByDescending(c => c.Cpf),
            EnumOrderByCustomer.Email => query.OrderByDescending(c => c.Email),
            EnumOrderByCustomer.CreationDate => query.OrderByDescending(c => c.CreationDate),
            _ => query.OrderByDescending(c => c.Id)
        };
    }
}