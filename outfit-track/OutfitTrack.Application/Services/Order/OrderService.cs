using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Application.Services;

public class OrderService(IUnitOfWork unitOfWork, ICustomerRepository customerRepository, IProductRepository productRepository, IOrderItemRepository orderItemRepository) : BaseService<IOrderRepository, InputCreateOrder, InputUpdateOrder, Order, OutputOrder, InputFilterOrder>(unitOfWork), IOrderService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;

    public OutputOrder? GetByNumber(long number)
    {
        var order = _repository!.Get(x => x.Number == number);
        return order != null ? FromEntityToOutput(order) : null;
    }

    public OutputMetricsOrder GetMetrics(DateTime start, DateTime end)
    {
        return _repository!.GetMetrics(start, end);
    }

    public OutputSalesMetricsOrder GetSalesMetrics(DateTime start, DateTime end)
    {
        return _repository!.GetSalesMetrics(start, end);
    }

    public override OutputOrder Create(InputCreateOrder inputCreate)
    {
        ValidateCustomerExists(inputCreate.CustomerId);
        ValidateProductsExist(inputCreate.ListCreatedItem);

        return ExecuteWithCommit(() =>
        {
            var order = new Order(
                inputCreate.CustomerId,
                EnumStatusOrder.Pending,
                null,
                _repository!.GetNextNumber(),
                inputCreate.Observation,
                null,
                null);

            var createdOrder = _repository.Create(order);

            _unitOfWork!.Commit();

            var orderItems = CreateOrderItems(inputCreate.ListCreatedItem!, createdOrder!.Id!.Value);
            createdOrder.SetProperty(nameof(Order.ListOrderItem), orderItems);

            return FromEntityToOutput(createdOrder);
        });
    }

    public override OutputOrder Update(long id, InputUpdateOrder inputUpdate)
    {
        var order = _repository!.Get(x => x.Id == id) ??
            throw new KeyNotFoundException("Pedido não encontrado.");

        ValidateOrderCanBeUpdated(order);

        return ExecuteWithCommit(() =>
        {
            var existingItems = order.ListOrderItem!.ToList();
            ProcessDeletedItems(inputUpdate.ListDeletedItem, existingItems);
            ProcessUpdatedItems(inputUpdate.ListUpdatedItem, existingItems);
            ProcessCreatedItems(inputUpdate.ListCreatedItem, order.Id!.Value, existingItems);

            order.SetProperty(nameof(Order.ListOrderItem), existingItems);
            order.SetProperty(nameof(Order.Observation), inputUpdate.Observation);
            UpdateOrderStatus(order, existingItems);

            _repository!.Update(order);
            return FromEntityToOutput(order);
        });
    }

    public bool Close(long id)
    {
        var order = _repository!.Get(x => x.Id == id) ??
            throw new KeyNotFoundException("Pedido não encontrado.");

        ValidateOrderCanBeClosed(order);

        ExecuteWithCommit(() =>
        {
            order.SetProperty(nameof(Order.Status), EnumStatusOrder.Closed);
            order.SetProperty(nameof(Order.ClosingDate), DateTime.UtcNow);
            _repository!.Update(order);
        });

        return true;
    }

    #region Business Validations
    private void ValidateCustomerExists(long? customerId)
    {
        var customer = _customerRepository.Get(x => x.Id == customerId);
        if (customer == null)
            throw new KeyNotFoundException("Não foi encontrado nenhum cliente correspondente a este Id.");
    }

    private void ValidateProductsExist(List<InputCreateOrderItem>? items)
    {
        if (items == null)
            return;

        foreach (var item in items)
        {
            var product = _productRepository.Get(x => x.Id == item.ProductId)
                ?? throw new KeyNotFoundException($"Não foi encontrado nenhum produto correspondente a este Id.");
        }
    }

    private static void ValidateOrderCanBeUpdated(Order order)
    {
        if (order.Status == EnumStatusOrder.Closed)
            throw new InvalidOperationException("Condicional finalizado!");
    }

    private static void ValidateOrderCanBeClosed(Order order)
    {
        if (order.ListOrderItem?.Any(x => x.Status == EnumStatusOrderItem.InProgress) == true)
            throw new InvalidOperationException("Há itens do pedido que estão com status 'Em andamento'");
    }
    #endregion

    #region Order Item Operations
    private List<OrderItem> CreateOrderItems(List<InputCreateOrderItem> items, long orderId)
    {
        int count = 1;
        var orderItems = items.Select(i => new OrderItem(
            count++,
            orderId,
            i.ProductId,
            i.Variation,
            EnumStatusOrderItem.InProgress,
            null,
            null))
            .ToList();

        foreach (var item in orderItems)
        {
            _orderItemRepository.Create(item);
        }

        return orderItems;
    }

    private void ProcessDeletedItems(List<long>? deletedItems, List<OrderItem> existingItems)
    {
        if (deletedItems == null)
            return;

        var itemsToRemove = new List<OrderItem>();
        foreach (var idItem in deletedItems)
        {
            var itemToDelete = existingItems.FirstOrDefault(x => x.Id == idItem);
            if (itemToDelete != null)
            {
                _orderItemRepository.Delete(itemToDelete);
                itemsToRemove.Add(itemToDelete);
            }
        }

        foreach (var item in itemsToRemove)
        {
            existingItems.Remove(item);
        }
    }

    private static void ProcessUpdatedItems(List<InputIdentityUpdateOrderItem>? updatedItems, List<OrderItem> existingItems)
    {
        if (updatedItems == null)
            return;

        foreach (var updateItem in updatedItems)
        {
            var itemToUpdate = existingItems.FirstOrDefault(x => x.Id == updateItem.Id);
            if (itemToUpdate != null)
            {
                itemToUpdate.SetProperty(nameof(OrderItem.Variation), updateItem.InputUpdate!.Variation);
                itemToUpdate.SetProperty(nameof(OrderItem.Status), updateItem.InputUpdate.Status);
                itemToUpdate.SetProperty(nameof(OrderItem.ChangeDate), DateTime.UtcNow);
            }
        }
    }

    private void ProcessCreatedItems(List<InputCreateOrderItem>? createdItems, long orderId, List<OrderItem> existingItems)
    {
        if (createdItems == null)
            return;

        int nextItemNumber = existingItems.Count + 1;
        var newItems = createdItems.Select(createItem => new OrderItem(nextItemNumber++, orderId,
            createItem.ProductId, createItem.Variation,
            EnumStatusOrderItem.InProgress, null, null)).ToList();

        foreach (var newItem in newItems)
        {
            _orderItemRepository.Create(newItem);
            existingItems.Add(newItem);
        }
    }

    private static void UpdateOrderStatus(Order order, List<OrderItem> existingItems)
    {
        if (existingItems.All(x => x.Status != EnumStatusOrderItem.InProgress))
            order.SetProperty(nameof(Order.Status), EnumStatusOrder.AwaitingClosure);
        else if (order.Status == EnumStatusOrder.AwaitingClosure &&
                 existingItems.Any(x => x.Status == EnumStatusOrderItem.InProgress))
            order.SetProperty(nameof(Order.Status), EnumStatusOrder.Pending);
    }
    #endregion

    #region Value Calculations
    private static decimal CalculateTotalValue(List<OrderItem> orderItems)
    {
        if (orderItems?.Count > 0)
        {
            return orderItems
                .Where(item => item.Product?.Price.HasValue == true)
                .Sum(item => item.Product!.Price!.Value);
        }
        return 0;
    }

    private static decimal? CalculateBilledValue(List<OrderItem> orderItems)
    {
        if (orderItems?.Count > 0)
        {
            return orderItems
                .Where(item => item.Status == EnumStatusOrderItem.Bought && item.Product?.Price.HasValue == true)
                .Sum(item => item.Product!.Price!.Value);
        }
        return 0;
    }
    #endregion

    #region Mappers
    public override OutputOrder FromEntityToOutput(Order entity)
    {
        var output = new OutputOrder
        {
            Id = entity.Id ?? 0,
            CreationDate = entity.CreationDate ?? DateTime.MinValue,
            ChangeDate = entity.ChangeDate,
            Number = entity.Number,
            CustomerId = entity.CustomerId,
            Status = entity.Status,
            ClosingDate = entity.ClosingDate,
            Observation = entity.Observation
        };

        if (entity.Customer != null)
            output.Customer = MapCustomerToOutput(entity.Customer);

        if (entity.ListOrderItem != null)
        {
            output.ListOrderItem = entity.ListOrderItem.Select(MapOrderItemToOutput).ToList();
            output.TotalValue = CalculateTotalValue(entity.ListOrderItem);
            output.BilledValue = CalculateBilledValue(entity.ListOrderItem);
        }

        return output;
    }

    public override Order FromInputCreateToEntity(InputCreateOrder inputCreate)
    {
        var entity = new Order();
        entity.SetProperty(nameof(Order.CustomerId), inputCreate.CustomerId);
        entity.SetProperty(nameof(Order.Status), EnumStatusOrder.Pending);
        entity.SetProperty(nameof(Order.Observation), inputCreate.Observation);
        return entity;
    }

    private static OutputCustomer MapCustomerToOutput(Customer customer)
    {
        return new OutputCustomer
        {
            Id = customer.Id ?? 0,
            CreationDate = customer.CreationDate ?? DateTime.MinValue,
            ChangeDate = customer.ChangeDate,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            BirthDate = customer.BirthDate,
            Cpf = customer.Cpf,
            Street = customer.Street,
            Complement = customer.Complement,
            Neighborhood = customer.Neighborhood,
            Number = customer.Number,
            CityName = customer.CityName,
            StateAbbreviation = customer.StateAbbreviation,
            PostalCode = customer.PostalCode,
            Rg = customer.Rg,
            MobilePhoneNumber = customer.MobilePhoneNumber,
            Email = customer.Email
        };
    }

    private static OutputOrderItem MapOrderItemToOutput(OrderItem item)
    {
        return new OutputOrderItem
        {
            Id = item.Id ?? 0,
            CreationDate = item.CreationDate ?? DateTime.MinValue,
            ChangeDate = item.ChangeDate,
            Item = item.Item,
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            Variation = item.Variation,
            Status = item.Status,
            Product = item.Product != null ? new OutputProduct
            {
                Id = item.Product.Id ?? 0,
                CreationDate = item.Product.CreationDate ?? DateTime.MinValue,
                ChangeDate = item.Product.ChangeDate,
                Code = item.Product.Code,
                Description = item.Product.Description,
                Price = item.Product.Price,
                Brand = item.Product.Brand,
                Category = item.Product.Category
            } : null
        };
    }
    #endregion
}