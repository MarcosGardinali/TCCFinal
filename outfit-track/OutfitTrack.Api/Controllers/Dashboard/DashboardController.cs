using Microsoft.AspNetCore.Mvc;
using OutfitTrack.Application.ApiManagement;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Api.Controllers;

[Route("api/[controller]")]
public class DashboardController(IApiDataService apiDataService, IOrderService orderService, ICustomerService customerService, IProductService productService) : BaseController<IOrderService, InputCreateOrder, InputUpdateOrder, OutputOrder, InputFilterOrder>(apiDataService, orderService)
{
    private readonly IOrderService _orderService = orderService;
    private readonly ICustomerService _customerService = customerService;
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Retorna todos os dados do dashboard (cards, gráficos e mapa) para o mês atual
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponseApi<OutputDashboard>>> GetCurrentMonth()
    {
        try
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1).AddTicks(-1);

            var orderMetrics = _orderService.GetMetrics(start, end);
            var salesMetrics = _orderService.GetSalesMetrics(start, end);
            var customerMetrics = _customerService.GetMetrics();

            var filter = new InputFilterOrder { DateFrom = start, DateTo = end };
            var ordersPage = _orderService.GetAllByFilter(filter, 1, 1000);
            var orders = ordersPage?.Items ?? new List<OutputOrder>();
            var activeOrders = orders.Where(o => o.Status == EnumStatusOrder.Pending || o.Status == EnumStatusOrder.AwaitingClosure).ToList();

            // Garantir que cada order ativo contenha os dados do cliente para o mapa/popup
            foreach (var ord in activeOrders)
            {
                if (ord.Customer == null && ord.CustomerId.HasValue)
                {
                    try
                    {
                        ord.Customer = _customerService.Get(ord.CustomerId.Value);
                    }
                    catch { /* ignore individual failures */ }
                }
            }

            var totalProducts = _productService.GetAllByFilter(new InputFilterProduct(), 1, 1)?.TotalItems ?? 0;

            var dashboard = new OutputDashboard
            {
                OrderMetrics = orderMetrics,
                SalesMetrics = salesMetrics,
                CustomerMetrics = customerMetrics,
                TotalProducts = totalProducts,
                ActiveOrders = activeOrders
            };

            return await ResponseAsync(dashboard);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
}
