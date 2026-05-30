using Moq;
using OutfitTrack.Application.Services;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Testes unitários para o OrderService
/// Testa todas as validações e cenários de exceção do serviço
/// </summary>
public class OrderServiceTests
{
    #region Helper Methods

    /// <summary>
    /// Cria e configura os mocks básicos necessários para os testes
    /// </summary>
    /// <returns>Mocks configurados</returns>
    private static (Mock<IUnitOfWork> mockUnitOfWork, Mock<IOrderRepository> mockRepository,
        Mock<ICustomerRepository> mockCustomerRepository, Mock<IProductRepository> mockProductRepository,
        Mock<IOrderItemRepository> mockOrderItemRepository) CreateBasicMocks()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderItemRepository, OrderItem, object>())
            .Returns(mockOrderItemRepository.Object);

        return (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);
    }

    /// <summary>
    /// Cria um serviço OrderService com os mocks configurados
    /// </summary>
    /// <param name="mocks">Mocks configurados</param>
    /// <returns>Instância do OrderService</returns>
    private static OrderService CreateOrderService(
        Mock<IUnitOfWork> mockUnitOfWork,
        Mock<ICustomerRepository> mockCustomerRepository,
        Mock<IProductRepository> mockProductRepository,
        Mock<IOrderItemRepository> mockOrderItemRepository)
    {
        return new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);
    }

    /// <summary>
    /// Cria um pedido existente para testes
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="status">Status do pedido</param>
    /// <param name="items">Itens do pedido (opcional)</param>
    /// <returns>Pedido configurado</returns>
    private static Order CreateExistingOrder(long orderId, EnumStatusOrder status, List<OrderItem>? items = null)
    {
        var order = new Order(1L, status, null, 1000L, "Observation", null, null);
        order.SetProperty(nameof(Order.Id), orderId);

        if (items != null)
        {
            order.SetProperty(nameof(Order.ListOrderItem), items);
        }

        return order;
    }

    /// <summary>
    /// Cria itens de pedido para testes
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="count">Número de itens a criar</param>
    /// <returns>Lista de itens de pedido configurados</returns>
    private static List<OrderItem> CreateOrderItems(long orderId, int count)
    {
        var items = new List<OrderItem>();

        for (int i = 1; i <= count; i++)
        {
            var item = new OrderItem(i, orderId, (long)i, $"Variation {i}", EnumStatusOrderItem.InProgress, null, null);
            item.SetProperty(nameof(OrderItem.Id), (long)i);
            items.Add(item);
        }

        return items;
    }

    #endregion

    #region Create Method Tests
    /// <summary>
    /// Testa que o método Create lança KeyNotFoundException quando o cliente não existe
    /// </summary>
    [Fact]
    public void Create_Should_Throw_KeyNotFoundException_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var inputCreate = new InputCreateOrder(1L, "Observation",
        [
            new(1L, "Variation")
        ]);

        // Cliente não existe
        mockCustomerRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Create(inputCreate));
        Assert.Equal("Não foi encontrado nenhum cliente correspondente a este Id.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Create lança KeyNotFoundException quando um produto não existe
    /// </summary>
    [Fact]
    public void Create_Should_Throw_KeyNotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var inputCreate = new InputCreateOrder(1L, "Observation", new List<InputCreateOrderItem>
        {
            new InputCreateOrderItem(1L, "Variation"),
            new InputCreateOrderItem(2L, "Variation 2")
        });

        // Cliente existe
        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());
        mockCustomerRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        // Primeiro produto existe
        var existingProduct1 = new Product("PROD001", "Product 1", 99.99m, "Brand 1", "Category 1", new List<OrderItem>());
        mockProductRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct1)
            .Verifiable();

        // Segundo produto não existe
        mockProductRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Create(inputCreate));
        Assert.Equal("Não foi encontrado nenhum produto correspondente a este Id.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Create cria um pedido com sucesso quando o cliente e produtos existem
    /// </summary>
    [Fact]
    public void Create_Should_Create_Order_When_Customer_And_Products_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var inputCreate = new InputCreateOrder(1L, "Observation", new List<InputCreateOrderItem>
        {
            new InputCreateOrderItem(1L, "Variation 1"),
            new InputCreateOrderItem(2L, "Variation 2")
        });

        // Cliente existe
        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());
        mockCustomerRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        // Produtos existem
        var existingProduct1 = new Product("PROD001", "Product 1", 99.99m, "Brand 1", "Category 1", new List<OrderItem>());
        var existingProduct2 = new Product("PROD002", "Product 2", 149.99m, "Brand 2", "Category 2", new List<OrderItem>());
        mockProductRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct1)
            .Verifiable();
        mockProductRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct2)
            .Verifiable();

        // Pedido a ser criado
        var newOrder = new Order(1L, EnumStatusOrder.Pending, null, 1000L, "Observation", null, null);
        newOrder.SetProperty(nameof(Order.Id), 1L);

        mockRepository.Setup(r => r.GetNextNumber())
            .Returns(1000L);

        mockRepository.Setup(r => r.Create(It.IsAny<Order>()))
            .Callback<Order>(o => o.SetProperty(nameof(Order.Id), 1L))
            .Returns(newOrder);

        mockOrderItemRepository.Setup(r => r.Create(It.IsAny<OrderItem>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Create(inputCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000L, result.Number);
        Assert.Equal(EnumStatusOrder.Pending, result.Status);
        mockRepository.Verify(r => r.Create(It.IsAny<Order>()), Times.Once);
        mockOrderItemRepository.Verify(r => r.Create(It.IsAny<OrderItem>()), Times.Exactly(2));
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Exactly(2));
    }

    #endregion

    #region Update Method Tests

    /// <summary>
    /// Testa que o método Update lança KeyNotFoundException quando o pedido não existe
    /// </summary>
    [Fact]
    public void Update_Should_Throw_KeyNotFoundException_When_Order_Does_Not_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;
        var inputUpdate = new InputUpdateOrder(null, new List<InputCreateOrderItem>(), new List<InputIdentityUpdateOrderItem>(), new List<long>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns((Order?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Update(orderId, inputUpdate));
        Assert.Equal("Pedido não encontrado.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update lança InvalidOperationException quando o pedido está fechado
    /// </summary>
    [Fact]
    public void Update_Should_Throw_InvalidOperationException_When_Order_Is_Closed()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;
        var existingOrder = new Order(1L, EnumStatusOrder.Closed, null, 1000L, "Observation", null, null);
        existingOrder.SetProperty(nameof(Order.Id), orderId);

        var inputUpdate = new InputUpdateOrder(null, new List<InputCreateOrderItem>(), new List<InputIdentityUpdateOrderItem>(), new List<long>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Update(orderId, inputUpdate));
        Assert.Equal("Condicional finalizado!", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update atualiza um pedido com sucesso quando o pedido existe e não está fechado
    /// </summary>
    [Fact]
    public void Update_Should_Update_Order_When_Order_Exists_And_Is_Not_Closed()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var orderId = 1L;
        var existingOrder = CreateExistingOrder(orderId, EnumStatusOrder.Pending, CreateOrderItems(orderId, 2));

        var inputUpdate = new InputUpdateOrder(
            null, // Observation
            new List<InputCreateOrderItem>(), // Nenhum item novo
            new List<InputIdentityUpdateOrderItem>(), // Nenhum item para atualizar
            new List<long> { 1 } // Deletar item com ID 1
        );

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        mockOrderItemRepository.Setup(r => r.Delete(It.IsAny<OrderItem>()))
            .Verifiable();

        mockRepository.Setup(r => r.Update(It.IsAny<Order>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Update(orderId, inputUpdate);

        // Assert
        Assert.NotNull(result);
        mockOrderItemRepository.Verify(r => r.Delete(It.IsAny<OrderItem>()), Times.Once);
        mockRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Close Method Tests

    /// <summary>
    /// Testa que o método Close lança KeyNotFoundException quando o pedido não existe
    /// </summary>
    [Fact]
    public void Close_Should_Throw_KeyNotFoundException_When_Order_Does_Not_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns((Order?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Close(orderId));
        Assert.Equal("Pedido não encontrado.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Close lança KeyNotFoundException quando há itens em andamento
    /// </summary>
    [Fact]
    public void Close_Should_Throw_KeyNotFoundException_When_Items_Are_In_Progress()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;
        var existingOrder = new Order(1L, EnumStatusOrder.AwaitingClosure, null, 1000L, "Observation", null, null);
        existingOrder.SetProperty(nameof(Order.Id), orderId);

        // Itens do pedido com status em andamento
        var existingItems = new List<OrderItem>
        {
            new(1, orderId, 1L, "Variation 1", EnumStatusOrderItem.InProgress, null, null),
            new(2, orderId, 2L, "Variation 2", EnumStatusOrderItem.Bought, null, null)
        };
        existingOrder.SetProperty(nameof(Order.ListOrderItem), existingItems);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Close(orderId));
        Assert.Equal("Há itens do pedido que estão com status 'Em andamento'", exception.Message);
    }

    /// <summary>
    /// Testa que o método Close fecha um pedido com sucesso quando todos os itens estão fechados
    /// </summary>
    [Fact]
    public void Close_Should_Close_Order_When_All_Items_Are_Closed()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;
        var existingOrder = new Order(1L, EnumStatusOrder.AwaitingClosure, null, 1000L, "Observation", null, null);
        existingOrder.SetProperty(nameof(Order.Id), orderId);

        // Itens do pedido todos fechados
        var existingItems = new List<OrderItem>
        {
            new OrderItem(1, orderId, 1L, "Variation 1", EnumStatusOrderItem.Bought, null, null),
            new OrderItem(2, orderId, 2L, "Variation 2", EnumStatusOrderItem.Bought, null, null)
        };
        existingOrder.SetProperty(nameof(Order.ListOrderItem), existingItems);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        mockRepository.Setup(r => r.Update(It.IsAny<Order>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Close(orderId);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region GetByNumber Method Tests

    /// <summary>
    /// Testa que o método GetByNumber retorna null quando o pedido não existe
    /// </summary>
    [Fact]
    public void GetByNumber_Should_Return_Null_When_Order_Does_Not_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderNumber = 1000L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns((Order?)null);

        // Act
        var result = service.GetByNumber(orderNumber);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetByNumber retorna um pedido quando o pedido existe
    /// </summary>
    [Fact]
    public void GetByNumber_Should_Return_Order_When_Order_Exists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderNumber = 1000L;
        var existingOrder = new Order(1L, EnumStatusOrder.Pending, null, orderNumber, "Observation", null, null);
        existingOrder.SetProperty(nameof(Order.Id), 1L);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        // Act
        var result = service.GetByNumber(orderNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderNumber, result.Number);
        Assert.Equal(1L, result.Id);
    }

    #endregion

    #region Get Method Tests

    /// <summary>
    /// Testa que o método Get retorna null quando o pedido não existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Null_When_Order_Does_Not_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns((Order?)null);

        // Act
        var result = service.Get(orderId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método Get retorna um pedido quando o pedido existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Order_When_Order_Exists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orderId = 1L;
        var existingOrder = new Order(1L, EnumStatusOrder.Pending, null, 1000L, "Observation", null, null);
        existingOrder.SetProperty(nameof(Order.Id), orderId);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>()))
            .Returns(existingOrder);

        // Act
        var result = service.Get(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000L, result.Number);
        Assert.Equal(orderId, result.Id);
    }

    #endregion

    #region GetAllByFilter Method Tests

    /// <summary>
    /// Testa que o método GetAllByFilter retorna null quando não há pedidos
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Null_When_No_Orders_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterOrder>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((PaginatedResult<Order>?)null);

        // Act
        var result = service.GetAllByFilter(new InputFilterOrder(), 1, 10);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter retorna uma lista de pedidos quando há pedidos
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Orders_When_Orders_Exist()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockRepository = new Mock<IOrderRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockProductRepository = new Mock<IProductRepository>();
        var mockOrderItemRepository = new Mock<IOrderItemRepository>();

        mockUnitOfWork.Setup(uow => uow.GetRepository<IOrderRepository, Order, InputFilterOrder>())
            .Returns(mockRepository.Object);

        var service = new OrderService(mockUnitOfWork.Object, mockCustomerRepository.Object, mockProductRepository.Object, mockOrderItemRepository.Object);

        var orders = new List<Order>
        {
            new Order(1L, EnumStatusOrder.Pending, null, 1000L, "Observation 1", null, null),
            new Order(2L, EnumStatusOrder.Closed, null, 1001L, "Observation 2", null, null)
        };

        var paginatedResult = new PaginatedResult<Order>
        {
            Items = orders,
            TotalItems = 2,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterOrder>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(new InputFilterOrder(), 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items?.Count() ?? 0);
    }

    #endregion

    #region Filter and Order Tests

    /// <summary>
    /// Testa que o método GetAllByFilter filtra corretamente por Status
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Filter_By_Status()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var filter = new InputFilterOrder { Status = EnumStatusOrder.Pending };

        var orders = new List<Order>
        {
            new Order(1L, EnumStatusOrder.Pending, null, 1001L, "Observation 1", null, null),
            new Order(2L, EnumStatusOrder.Pending, null, 1002L, "Observation 2", null, null)
        };

        var paginatedResult = new PaginatedResult<Order>
        {
            Items = orders,
            TotalItems = 2,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(filter, 1, 10))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(filter, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter ordena corretamente por Number descendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_Number_Descending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var filter = new InputFilterOrder
        {
            OrderBy = EnumOrderByOrder.Number,
            OrderByDescending = true
        };

        var orders = new List<Order>
        {
            new Order(1L, EnumStatusOrder.Pending, null, 1001L, "Observation 1", null, null),
            new Order(2L, EnumStatusOrder.Pending, null, 1003L, "Observation 2", null, null),
            new Order(3L, EnumStatusOrder.Pending, null, 1002L, "Observation 3", null, null)
        };

        var paginatedResult = new PaginatedResult<Order>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(filter, 1, 10))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(filter, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter ordena corretamente por CreationDate ascendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_CreationDate_Ascending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var filter = new InputFilterOrder
        {
            OrderBy = EnumOrderByOrder.CreationDate,
            OrderByDescending = false
        };

        var orders = new List<Order>
        {
            new Order(1L, EnumStatusOrder.Pending, null, 1001L, "Observation 1", null, null),
            new Order(2L, EnumStatusOrder.Pending, null, 1002L, "Observation 2", null, null),
            new Order(3L, EnumStatusOrder.Pending, null, 1003L, "Observation 3", null, null)
        };

        var paginatedResult = new PaginatedResult<Order>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(filter, 1, 10))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(filter, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter filtra e ordena corretamente combinados
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Filter_And_Order_Combined()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var filter = new InputFilterOrder
        {
            Status = EnumStatusOrder.Pending,
            OrderBy = EnumOrderByOrder.Number,
            OrderByDescending = false
        };

        var orders = new List<Order>
        {
            new Order(1L, EnumStatusOrder.Pending, null, 1003L, "Observation 1", null, null),
            new Order(2L, EnumStatusOrder.Pending, null, 1001L, "Observation 2", null, null),
            new Order(3L, EnumStatusOrder.Pending, null, 1002L, "Observation 3", null, null)
        };

        var paginatedResult = new PaginatedResult<Order>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(filter, 1, 10))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(filter, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.Items?.Count() ?? 0);
    }

    #endregion

    #region Metrics Method Tests

    /// <summary>
    /// Testa que o método GetMetricsByDateRange retorna as métricas do repositório para o período
    /// </summary>
    [Fact]
    public void GetMetricsByDateRange_Should_Return_Metrics_From_Repository()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var start = DateTime.Now.AddDays(-7);
        var end = DateTime.Now;
        var expectedMetrics = new OutputMetricsOrder { TotalOrders = 5, TotalRevenue = 500m };

        mockRepository.Setup(r => r.GetMetrics(start, end)).Returns(expectedMetrics);

        // Act
        var result = service.GetMetrics(start, end);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMetrics.TotalOrders, result.TotalOrders);
        Assert.Equal(expectedMetrics.TotalRevenue, result.TotalRevenue);
        mockRepository.Verify(r => r.GetMetrics(start, end), Times.Once);
    }

    /// <summary>
    /// Testa que o método GetSalesMetrics retorna as métricas de vendas do repositório
    /// </summary>
    [Fact]
    public void GetSalesMetrics_Should_Return_Metrics_From_Repository()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository, mockCustomerRepository, mockProductRepository, mockOrderItemRepository) = CreateBasicMocks();
        var service = CreateOrderService(mockUnitOfWork, mockCustomerRepository, mockProductRepository, mockOrderItemRepository);

        var start = DateTime.UtcNow.AddDays(-7);
        var end = DateTime.UtcNow;
        var expectedMetrics = new OutputSalesMetricsOrder { TotalRevenue = 1000m, TotalSalesCount = 10 };

        mockRepository.Setup(r => r.GetSalesMetrics(start, end)).Returns(expectedMetrics);

        // Act
        var result = service.GetSalesMetrics(start, end);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMetrics.TotalRevenue, result.TotalRevenue);
        Assert.Equal(expectedMetrics.TotalSalesCount, result.TotalSalesCount);
        mockRepository.Verify(r => r.GetSalesMetrics(start, end), Times.Once);
    }

    #endregion
}