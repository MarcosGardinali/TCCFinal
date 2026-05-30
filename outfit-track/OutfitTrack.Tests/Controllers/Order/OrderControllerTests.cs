using Microsoft.AspNetCore.Mvc;
using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Arguments;
using OutfitTrack.Arguments.Enums;

namespace OutfitTrack.Tests.Controllers;

/// <summary>
/// Testes unitários para o OrderController
/// </summary>
public class OrderControllerTests : BaseControllerTests
{
    // Mock do serviço de pedido - usado para simular o comportamento da camada de negócio
    private readonly Mock<IOrderService> _mockService;
    // Instância do controlador que será testado
    private readonly OrderController _controller;

    /// <summary>
    /// Construtor que configura os mocks e instancia o controlador
    /// </summary>
    public OrderControllerTests()
    {
        _mockService = new Mock<IOrderService>();
        _controller = new OrderController(MockApiDataService.Object, _mockService.Object);
    }

    /// <summary>
    /// Testa o endpoint GetByNumber quando o pedido existe
    /// Deve retornar OK (200) com os dados do pedido
    /// </summary>
    [Fact]
    public async Task GetByNumber_Should_Return_Ok_When_Order_Exists()
    {
        // Arrange - Preparação do teste
        var orderNumber = 1001L;
        var expectedOrder = new OutputOrder
        {
            Id = 1,
            Number = orderNumber,
            Status = EnumStatusOrder.Pending
        };

        // Configura o mock para retornar um pedido quando o método GetByNumber for chamado
        _mockService.Setup(x => x.GetByNumber(orderNumber)).Returns(expectedOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByNumber(orderNumber);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedOrder);
    }

    /// <summary>
    /// Testa o endpoint GetByNumber quando o pedido não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task GetByNumber_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var orderNumber = 1001L;
        OutputOrder? nullOrder = null;
        // Configura o mock para retornar null quando o método GetByNumber for chamado
        _mockService.Setup(x => x.GetByNumber(orderNumber)).Returns(nullOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByNumber(orderNumber);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint GetByNumber quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetByNumber_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var orderNumber = 1001L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método GetByNumber for chamado
        _mockService.Setup(x => x.GetByNumber(orderNumber)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetByNumber(orderNumber);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Close quando o pedido é fechado com sucesso
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task Close_Should_Return_Ok_When_Order_Is_Closed_Successfully()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        // Configura o mock para retornar true quando o método Close for chamado
        _mockService.Setup(x => x.Close(orderId)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.Close(orderId);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint Close quando o pedido não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Close_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        // Configura o mock para lançar KeyNotFoundException quando o método Close for chamado
        _mockService.Setup(x => x.Close(orderId)).Throws(new KeyNotFoundException("Item não encontrado."));

        // Act - Execução do método a ser testado
        var result = await _controller.Close(orderId);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint Close quando há itens do pedido com status 'Em andamento'
    /// Deve retornar BadRequest (400) com mensagem de erro específica
    /// </summary>
    [Fact]
    public async Task Close_Should_Return_BadRequest_When_Order_Items_Are_In_Progress()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        var exceptionMessage = "Há itens do pedido que estão com status 'Em andamento'";
        // Configura o mock para lançar InvalidOperationException quando o método Close for chamado
        _mockService.Setup(x => x.Close(orderId)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Close(orderId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Close quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Close_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Close for chamado
        _mockService.Setup(x => x.Close(orderId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Close(orderId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o pedido existe e pode ser excluído
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_Ok_When_Order_Can_Be_Deleted()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        // Configura o mock para retornar true quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(orderId)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(orderId);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o pedido não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        // Configura o mock para lançar KeyNotFoundException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(orderId)).Throws(new KeyNotFoundException("Item não encontrado para exclusão."));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(orderId);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado para exclusão.");
    }

    /// <summary>
    /// Testa o endpoint Delete quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(orderId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(orderId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    #region Metrics Tests

    /// <summary>
    /// Testa o endpoint GetMetricsByDateRange
    /// </summary>
    [Fact]
    public async Task GetMetricsByDateRange_Should_Return_Ok_With_Metrics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var expectedMetrics = new OutputMetricsOrder
        {
            TotalOrders = 5,
            TotalRevenue = 500m,
            PendingOrdersCount = 2
        };
        _mockService.Setup(x => x.GetMetrics(startDate, endDate)).Returns(expectedMetrics);

        // Act
        var result = await _controller.GetMetrics(startDate, endDate);

        // Assert
        AssertOkResult(result, expectedMetrics);
    }

    /// <summary>
    /// Testa o endpoint GetSalesMetrics
    /// </summary>
    [Fact]
    public async Task GetSalesMetrics_Should_Return_Ok_With_Metrics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var expectedMetrics = new OutputSalesMetricsOrder
        {
            TotalRevenue = 800m,
            TotalSalesCount = 8,
            AverageTicket = 100m,
            TotalItemsSoldCount = 15
        };
        _mockService.Setup(x => x.GetSalesMetrics(startDate, endDate)).Returns(expectedMetrics);

        // Act
        var result = await _controller.GetSalesMetrics(startDate, endDate);

        // Assert
        AssertOkResult(result, expectedMetrics);
    }

    #endregion

    /// <summary>
    /// Testa o endpoint Create quando o pedido é criado com sucesso
    /// Deve retornar Created (201) com os dados do pedido criado
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_Created_When_Order_Is_Created_Successfully()
    {
        // Arrange - Preparação do teste
        var listCreatedItem = new List<InputCreateOrderItem>
        {
            new(1, "Variation 1")
        };

        var inputCreate = new InputCreateOrder(1, null, listCreatedItem);

        var expectedOrder = new OutputOrder
        {
            Id = 1,
            CustomerId = 1,
            Observation = "Test order",
            Status = EnumStatusOrder.Pending
        };

        // Configura o mock para retornar o pedido criado quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Returns(expectedOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertCreatedResult(result, expectedOrder);
    }

    /// <summary>
    /// Testa o endpoint Create quando o cliente não existe
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Customer_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var listCreatedItem = new List<InputCreateOrderItem>
        {
            new(1, "Variation 1")
        };

        var inputCreate = new InputCreateOrder(1, null, listCreatedItem);

        var exceptionMessage = "Não foi encontrado nenhum cliente correspondente a este Id.";
        // Configura o mock para lançar KeyNotFoundException quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new KeyNotFoundException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Create quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var listCreatedItem = new List<InputCreateOrderItem>
        {
            new(1, "Variation 1")
        };

        var inputCreate = new InputCreateOrder(1, null, listCreatedItem);

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Get quando o pedido existe
    /// Deve retornar OK (200) com os dados do pedido
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_Ok_When_Order_Exists()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        var expectedOrder = new OutputOrder
        {
            Id = 1,
            Number = 1001L,
            Status = EnumStatusOrder.Pending
        };

        // Configura o mock para retornar um pedido quando o método Get for chamado
        _mockService.Setup(x => x.Get(orderId)).Returns(expectedOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(orderId);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedOrder);
    }

    /// <summary>
    /// Testa o endpoint Get quando o pedido não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        OutputOrder? nullOrder = null;
        // Configura o mock para retornar null quando o método Get for chamado
        _mockService.Setup(x => x.Get(orderId)).Returns(nullOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(orderId);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint Get quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var orderId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método Get for chamado
        _mockService.Setup(x => x.Get(orderId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Get(orderId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Update quando o pedido é atualizado com sucesso
    /// Deve retornar OK (200) com os dados do pedido atualizado
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_Ok_When_Order_Is_Updated_Successfully()
    {
        // Arrange - Preparação do teste
        var listUpdatedItem = new List<InputIdentityUpdateOrderItem>
        {
            new(1, new InputUpdateOrderItem("New variation", EnumStatusOrderItem.Bought))
        };

        var inputUpdate = new InputUpdateOrder("Updated order", null, listUpdatedItem, null);

        var expectedOrder = new OutputOrder
        {
            Id = 1,
            Number = 1001L,
            Observation = "Updated order",
            Status = EnumStatusOrder.Pending
        };

        // Configura o mock para retornar o pedido atualizado quando o método Update for chamado
        _mockService.Setup(x => x.Update(1, inputUpdate)).Returns(expectedOrder);

        // Act - Execução do método a ser testado
        var result = await _controller.Update(1, inputUpdate);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedOrder);
    }

    /// <summary>
    /// Testa o endpoint Update quando o pedido não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_NotFound_When_Order_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var listUpdatedItem = new List<InputIdentityUpdateOrderItem>
        {
            new(1, new InputUpdateOrderItem("New variation", EnumStatusOrderItem.Bought))
        };

        var inputUpdate = new InputUpdateOrder("Updated order", null, listUpdatedItem, null);

        // Configura o mock para lançar KeyNotFoundException quando o método Update for chamado
        _mockService.Setup(x => x.Update(1, inputUpdate)).Throws(new KeyNotFoundException("Id inválido ou inexistente."));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(1, inputUpdate);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Id inválido ou inexistente.");
    }

    /// <summary>
    /// Testa o endpoint Update quando o pedido já está fechado
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Order_Is_Closed()
    {
        // Arrange - Preparação do teste
        var listUpdatedItem = new List<InputIdentityUpdateOrderItem>
        {
            new(1, new InputUpdateOrderItem("New variation", EnumStatusOrderItem.Bought))
        };

        var inputUpdate = new InputUpdateOrder("Updated order", null, listUpdatedItem, null);

        var exceptionMessage = "Condicional finalizado!";
        // Configura o mock para lançar InvalidOperationException quando o método Update for chamado
        _mockService.Setup(x => x.Update(1, inputUpdate)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(1, inputUpdate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Update quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var listUpdatedItem = new List<InputIdentityUpdateOrderItem>
        {
            new(1, new InputUpdateOrderItem("New variation", EnumStatusOrderItem.Bought))
        };

        var inputUpdate = new InputUpdateOrder("Updated order", null, listUpdatedItem, null);

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Update for chamado
        _mockService.Setup(x => x.Update(1, inputUpdate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(1, inputUpdate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há pedidos cadastrados
    /// Deve retornar OK (200) com a lista paginada de pedidos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Orders_Exist()
    {
        // Arrange - Preparação do teste
        var pageNumber = 1;
        var pageSize = 10;
        var orders = new List<OutputOrder>
        {
            new OutputOrder { Id = 1, Number = 1001L, Status = EnumStatusOrder.Pending },
            new OutputOrder { Id = 2, Number = 1002L, Status = EnumStatusOrder.Pending }
        };

        var paginatedResult = new PaginatedResult<OutputOrder>
        {
            Items = orders,
            TotalItems = 2,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        var filter = new InputFilterOrder();
        // Configura o mock para retornar a lista paginada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputOrder>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(orders?.Count ?? 0, response.Result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterOrder();
        var pageNumber = 1;
        var pageSize = 10;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    #region Filter and Order Tests

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há filtro por Status
    /// Deve retornar OK (200) com a lista filtrada de pedidos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_By_Status()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterOrder { Status = EnumStatusOrder.Pending };
        var pageNumber = 1;
        var pageSize = 10;

        var orders = new List<OutputOrder>
        {
            new() { Id = 1, Number = 1001L, Status = EnumStatusOrder.Pending },
            new() { Id = 2, Number = 1002L, Status = EnumStatusOrder.Pending }
        };

        var paginatedResult = new PaginatedResult<OutputOrder>
        {
            Items = orders,
            TotalItems = 2,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        // Configura o mock para retornar a lista filtrada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputOrder>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(2, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, order => Assert.Equal(EnumStatusOrder.Pending, order.Status));
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por Number descendente
    /// Deve retornar OK (200) com a lista ordenada de pedidos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_Number_Descending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterOrder
        {
            OrderBy = EnumOrderByOrder.Number,
            OrderByDescending = true
        };
        var pageNumber = 1;
        var pageSize = 10;

        var orders = new List<OutputOrder>
        {
            new() { Id = 1, Number = 1003L, Status = EnumStatusOrder.Pending },
            new() { Id = 2, Number = 1002L, Status = EnumStatusOrder.Pending },
            new() { Id = 3, Number = 1001L, Status = EnumStatusOrder.Pending }
        };

        var paginatedResult = new PaginatedResult<OutputOrder>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        // Configura o mock para retornar a lista ordenada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputOrder>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);

        // Verifica se os itens estão ordenados corretamente (número descendente)
        var items = response.Result.Items!.ToList();
        Assert.Equal(1003L, items[0].Number);
        Assert.Equal(1002L, items[1].Number);
        Assert.Equal(1001L, items[2].Number);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por CreationDate ascendente
    /// Deve retornar OK (200) com a lista ordenada de pedidos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_CreationDate_Ascending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterOrder
        {
            OrderBy = EnumOrderByOrder.CreationDate,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        var orders = new List<OutputOrder>
        {
            new() { Id = 1, Number = 1001L, Status = EnumStatusOrder.Pending },
            new() { Id = 2, Number = 1002L, Status = EnumStatusOrder.Pending },
            new() { Id = 3, Number = 1003L, Status = EnumStatusOrder.Pending }
        };

        var paginatedResult = new PaginatedResult<OutputOrder>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        // Configura o mock para retornar a lista ordenada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputOrder>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há filtro combinado e ordenação
    /// Deve retornar OK (200) com a lista filtrada e ordenada de pedidos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_And_Ordering_Combined()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterOrder
        {
            Status = EnumStatusOrder.Pending,
            OrderBy = EnumOrderByOrder.Number,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        var orders = new List<OutputOrder>
        {
            new() { Id = 1, Number = 1001L, Status = EnumStatusOrder.Pending },
            new() { Id = 2, Number = 1002L, Status = EnumStatusOrder.Pending },
            new() { Id = 3, Number = 1003L, Status = EnumStatusOrder.Pending }
        };

        var paginatedResult = new PaginatedResult<OutputOrder>
        {
            Items = orders,
            TotalItems = 3,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        // Configura o mock para retornar a lista filtrada e ordenada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputOrder>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, order => Assert.Equal(EnumStatusOrder.Pending, order.Status));

        // Verifica se os itens estão ordenados corretamente por número ascendente
        var items = response.Result.Items!.ToList();
        Assert.Equal(1001L, items[0].Number);
        Assert.Equal(1002L, items[1].Number);
        Assert.Equal(1003L, items[2].Number);
    }

    #endregion
}