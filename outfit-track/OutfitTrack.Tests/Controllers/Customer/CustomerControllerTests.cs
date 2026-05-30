using Microsoft.AspNetCore.Mvc;
using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;

namespace OutfitTrack.Tests.Controllers;

/// <summary>
/// Testes unitários para o CustomerController
/// </summary>
public class CustomerControllerTests : BaseControllerTests
{
    // Mock do serviço de cliente - usado para simular o comportamento da camada de negócio
    private readonly Mock<ICustomerService> _mockService;
    // Instância do controlador que será testado
    private readonly CustomerController _controller;

    /// <summary>
    /// Construtor que configura os mocks e instancia o controlador
    /// </summary>
    public CustomerControllerTests()
    {
        _mockService = new Mock<ICustomerService>();
        _controller = new CustomerController(MockApiDataService.Object, _mockService.Object);
    }

    /// <summary>
    /// Testa o endpoint GetByCpf quando o cliente existe
    /// Deve retornar OK (200) com os dados do cliente
    /// </summary>
    [Fact]
    public async Task GetByCpf_Should_Return_Ok_When_Customer_Exists()
    {
        // Arrange - Preparação do teste
        var cpf = "12345678901";
        var expectedCustomer = new OutputCustomer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Cpf = cpf
        };

        // Configura o mock para retornar um cliente quando o método GetByCpf for chamado
        _mockService.Setup(x => x.GetByCpf(cpf)).Returns(expectedCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCpf(cpf);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedCustomer);
    }

    /// <summary>
    /// Testa o endpoint GetByCpf quando o cliente não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task GetByCpf_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var cpf = "12345678901";
        OutputCustomer? nullCustomer = null;
        // Configura o mock para retornar null quando o método GetByCpf for chamado
        _mockService.Setup(x => x.GetByCpf(cpf)).Returns(nullCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCpf(cpf);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint GetByCpf quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetByCpf_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var cpf = "12345678901";
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método GetByCpf for chamado
        _mockService.Setup(x => x.GetByCpf(cpf)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCpf(cpf);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o cliente existe e pode ser excluído
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_Ok_When_Customer_Can_Be_Deleted()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        // Configura o mock para retornar true quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(customerId)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(customerId);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o cliente não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        // Configura o mock para lançar KeyNotFoundException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(customerId)).Throws(new KeyNotFoundException("Item não encontrado para exclusão."));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(customerId);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado para exclusão.");
    }

    /// <summary>
    /// Testa o endpoint Delete quando o cliente possui vínculo com pedidos
    /// Deve retornar BadRequest (400) com mensagem de erro específica
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Customer_Has_Orders()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var exceptionMessage = "Esse cliente possui vínculo com pedidos";
        // Configura o mock para lançar InvalidOperationException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(customerId)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(customerId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Delete quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(customerId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(customerId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Create quando o cliente é criado com sucesso
    /// Deve retornar Created (201) com os dados do cliente criado
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_Created_When_Customer_Is_Created_Successfully()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateCustomer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901", "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "john.doe@example.com");

        var expectedCustomer = new OutputCustomer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Cpf = "12345678901",
            Email = "john.doe@example.com"
        };

        // Configura o mock para retornar o cliente criado quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Returns(expectedCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertCreatedResult(result, expectedCustomer);
    }

    /// <summary>
    /// Testa o endpoint Create quando o CPF já está cadastrado
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Cpf_Already_Exists()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateCustomer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901", "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "john.doe@example.com");

        var exceptionMessage = "Cpf '12345678901' já cadastrado.";
        // Configura o mock para lançar InvalidOperationException quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new InvalidOperationException(exceptionMessage));

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
        var inputCreate = new InputCreateCustomer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901", "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "john.doe@example.com");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Get quando o cliente existe
    /// Deve retornar OK (200) com os dados do cliente
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_Ok_When_Customer_Exists()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var expectedCustomer = new OutputCustomer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Cpf = "12345678901"
        };

        // Configura o mock para retornar um cliente quando o método Get for chamado
        _mockService.Setup(x => x.Get(customerId)).Returns(expectedCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(customerId);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedCustomer);
    }

    /// <summary>
    /// Testa o endpoint Get quando o cliente não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        OutputCustomer? nullCustomer = null;
        // Configura o mock para retornar null quando o método Get for chamado
        _mockService.Setup(x => x.Get(customerId)).Returns(nullCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(customerId);

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
        var customerId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método Get for chamado
        _mockService.Setup(x => x.Get(customerId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Get(customerId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Update quando o cliente é atualizado com sucesso
    /// Deve retornar OK (200) com os dados do cliente atualizado
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_Ok_When_Customer_Is_Updated_Successfully()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var inputUpdate = new InputUpdateCustomer("Jane", "Smith", DateTime.Now.AddYears(-30), "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "jane.smith@example.com");

        var expectedCustomer = new OutputCustomer
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Cpf = "12345678901",
            Email = "jane.smith@example.com"
        };

        // Configura o mock para retornar o cliente atualizado quando o método Update for chamado
        _mockService.Setup(x => x.Update(customerId, inputUpdate)).Returns(expectedCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.Update(customerId, inputUpdate);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedCustomer);
    }

    /// <summary>
    /// Testa o endpoint Update quando o cliente não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var inputUpdate = new InputUpdateCustomer("Jane", "Smith", DateTime.Now.AddYears(-30), "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "jane.smith@example.com");

        // Configura o mock para lançar KeyNotFoundException quando o método Update for chamado
        _mockService.Setup(x => x.Update(customerId, inputUpdate)).Throws(new KeyNotFoundException("Id inválido ou inexistente."));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(customerId, inputUpdate);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Id inválido ou inexistente.");
    }

    /// <summary>
    /// Testa o endpoint Update quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var customerId = 1L;
        var inputUpdate = new InputUpdateCustomer("Jane", "Smith", DateTime.Now.AddYears(-30), "Street", null, "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP, "12345678", "123456789", "1234567890123", "jane.smith@example.com");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Update for chamado
        _mockService.Setup(x => x.Update(customerId, inputUpdate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(customerId, inputUpdate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há clientes cadastrados
    /// Deve retornar OK (200) com a lista paginada de clientes
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Customers_Exist()
    {
        // Arrange - Preparação do teste
        var pageNumber = 1;
        var pageSize = 10;
        var customers = new List<OutputCustomer>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };

        var paginatedResult = new PaginatedResult<OutputCustomer>
        {
            Items = customers,
            TotalItems = 2,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        var filter = new InputFilterCustomer();
        // Configura o mock para retornar a lista paginada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputCustomer>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(customers?.Count() ?? 0, response.Result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var pageNumber = 1;
        var pageSize = 10;
        var exceptionMessage = "Database error";
        var filter = new InputFilterCustomer();
        // Configura o mock para lançar uma exceção quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    #region GetTopByOrders Tests

    /// <summary>
    /// Testa o endpoint GetTopByOrders quando há um cliente com mais pedidos
    /// Deve retornar OK (200) com o cliente que mais fez pedidos
    /// </summary>
    [Fact]
    public async Task GetTopByOrders_Should_Return_Ok_With_Top_Customer()
    {
        // Arrange - Preparação do teste
        var topCustomer = new OutputCustomer
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Cpf = "12345678901"
        };
        _mockService.Setup(x => x.GetTopByOrders()).Returns(topCustomer);

        // Act - Execução do método a ser testado
        var result = await _controller.GetTopByOrders();

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<OutputCustomer>;
        Assert.NotNull(response);
        Assert.NotNull(response.Result);
        Assert.Equal("John", response.Result.FirstName);
    }

    /// <summary>
    /// Testa o endpoint GetTopByOrders quando não há clientes
    /// Deve retornar NotFound (404) com mensagem apropriada
    /// </summary>
    [Fact]
    public async Task GetTopByOrders_Should_Return_NotFound_When_No_Customers()
    {
        // Arrange - Preparação do teste
        _mockService.Setup(x => x.GetTopByOrders()).Returns((OutputCustomer?)null);

        // Act - Execução do método a ser testado
        var result = await _controller.GetTopByOrders();

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(404, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<string>;
        Assert.NotNull(response);
        Assert.Equal("Nenhum cliente encontrado.", response.ErrorMessage);
    }

    /// <summary>
    /// Testa o endpoint GetTopByOrders quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetTopByOrders_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var exceptionMessage = "Database error";
        _mockService.Setup(x => x.GetTopByOrders()).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetTopByOrders();

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    #endregion

    #region Metrics Tests

    /// <summary>
    /// Testa o endpoint GetCompleteMetrics
    /// </summary>
    [Fact]
    public async Task GetCompleteMetrics_Should_Return_Ok_With_Metrics()
    {
        // Arrange
        var expectedMetrics = new OutputMetricsCustomer
        {
            TotalCustomers = 100,
            ActiveCustomersCount = 80,
            AverageAge = 35
        };
        _mockService.Setup(x => x.GetMetrics()).Returns(expectedMetrics);

        // Act
        var result = await _controller.GetMetrics();

        // Assert
        AssertOkResult(result, expectedMetrics);
    }

    #endregion

    #region Filter and Order Tests

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há filtro por FirstName
    /// Deve retornar OK (200) com a lista filtrada de clientes
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_By_FirstName()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterCustomer { Name = "John" };
        var pageNumber = 1;
        var pageSize = 10;

        var customers = new List<OutputCustomer>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe" },
            new() { Id = 2, FirstName = "John", LastName = "Smith" }
        };

        var paginatedResult = new PaginatedResult<OutputCustomer>
        {
            Items = customers,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputCustomer>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(2, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, customer => Assert.Equal("John", customer.FirstName));
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por FirstName ascendente
    /// Deve retornar OK (200) com a lista ordenada de clientes
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_FirstName_Ascending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterCustomer
        {
            OrderBy = EnumOrderByCustomer.FirstName,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        var customers = new List<OutputCustomer>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Doe" },
            new() { Id = 2, FirstName = "Bob", LastName = "Smith" },
            new() { Id = 3, FirstName = "Charlie", LastName = "Johnson" }
        };

        var paginatedResult = new PaginatedResult<OutputCustomer>
        {
            Items = customers,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputCustomer>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);

        // Verifica se os itens estão ordenados corretamente
        var items = response.Result.Items!.ToList();
        Assert.Equal("Alice", items[0].FirstName);
        Assert.Equal("Bob", items[1].FirstName);
        Assert.Equal("Charlie", items[2].FirstName);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por CreationDate descendente
    /// Deve retornar OK (200) com a lista ordenada de clientes
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_CreationDate_Descending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterCustomer
        {
            OrderBy = EnumOrderByCustomer.CreationDate,
            OrderByDescending = true
        };
        var pageNumber = 1;
        var pageSize = 10;

        var customers = new List<OutputCustomer>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Doe" },
            new() { Id = 2, FirstName = "Bob", LastName = "Smith" },
            new() { Id = 3, FirstName = "Charlie", LastName = "Johnson" }
        };

        var paginatedResult = new PaginatedResult<OutputCustomer>
        {
            Items = customers,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputCustomer>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há filtro combinado e ordenação
    /// Deve retornar OK (200) com a lista filtrada e ordenada de clientes
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_And_Ordering_Combined()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterCustomer
        {
            Name = "Smith",
            OrderBy = EnumOrderByCustomer.FirstName,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        var customers = new List<OutputCustomer>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Smith" },
            new() { Id = 2, FirstName = "Bob", LastName = "Smith" }
        };

        var paginatedResult = new PaginatedResult<OutputCustomer>
        {
            Items = customers,
            TotalItems = 2,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputCustomer>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(2, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, customer => Assert.Equal("Smith", customer.LastName));

        // Verifica se os itens estão ordenados corretamente
        var items = response.Result.Items!.ToList();
        Assert.Equal("Alice", items[0].FirstName);
        Assert.Equal("Bob", items[1].FirstName);
    }

    #endregion
}