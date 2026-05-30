using Microsoft.AspNetCore.Mvc;
using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;

namespace OutfitTrack.Tests.Controllers;

/// <summary>
/// Testes unitários para o ProductController
/// </summary>
public class ProductControllerTests : BaseControllerTests
{
    // Mock do serviço de produto - usado para simular o comportamento da camada de negócio
    private readonly Mock<IProductService> _mockService;
    // Instância do controlador que será testado
    private readonly ProductController _controller;

    /// <summary>
    /// Construtor que configura os mocks e instancia o controlador
    /// </summary>
    public ProductControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductController(MockApiDataService.Object, _mockService.Object);
    }

    /// <summary>
    /// Testa o endpoint GetByCode quando o produto existe
    /// Deve retornar OK (200) com os dados do produto
    /// </summary>
    [Fact]
    public async Task GetByCode_Should_Return_Ok_When_Product_Exists()
    {
        // Arrange - Preparação do teste
        var code = "PROD001";
        var expectedProduct = new OutputProduct
        {
            Id = 1,
            Code = code,
            Description = "Test Product",
            Price = 100.00m
        };

        // Configura o mock para retornar um produto quando o método GetByCode for chamado
        _mockService.Setup(x => x.GetByCode(code)).Returns(expectedProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCode(code);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedProduct);
    }

    /// <summary>
    /// Testa o endpoint GetByCode quando o produto não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task GetByCode_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var code = "PROD001";
        OutputProduct? nullProduct = null;
        // Configura o mock para retornar null quando o método GetByCode for chamado
        _mockService.Setup(x => x.GetByCode(code)).Returns(nullProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCode(code);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint GetByCode quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetByCode_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var code = "PROD001";
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método GetByCode for chamado
        _mockService.Setup(x => x.GetByCode(code)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetByCode(code);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o produto existe e pode ser excluído
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_Ok_When_Product_Can_Be_Deleted()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        // Configura o mock para retornar true quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(productId)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(productId);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o produto não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        // Configura o mock para lançar KeyNotFoundException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(productId)).Throws(new KeyNotFoundException("Item não encontrado para exclusão."));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(productId);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado para exclusão.");
    }

    /// <summary>
    /// Testa o endpoint Delete quando o produto possui vínculo com itens de pedido
    /// Deve retornar BadRequest (400) com mensagem de erro específica
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Product_Has_Order_Items()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        var exceptionMessage = "Esse produto possui vínculo com itens de pedido";
        // Configura o mock para lançar InvalidOperationException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(productId)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(productId);

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
        var productId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(productId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(productId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Create quando o produto é criado com sucesso
    /// Deve retornar Created (201) com os dados do produto criado
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_Created_When_Product_Is_Created_Successfully()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateProduct("PROD001", "Test Product", 100.00m, "Test Brand", "Test Category");

        var expectedProduct = new OutputProduct
        {
            Id = 1,
            Code = "PROD001",
            Description = "Test Product",
            Price = 100.00m
        };

        // Configura o mock para retornar o produto criado quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Returns(expectedProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertCreatedResult(result, expectedProduct);
    }

    /// <summary>
    /// Testa o endpoint Create quando o código já está cadastrado
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Code_Already_Exists()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateProduct("PROD001", "Test Product", 100.00m, "Test Brand", "Test Category");

        var exceptionMessage = "Código 'PROD001' já cadastrado.";
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
        var inputCreate = new InputCreateProduct("PROD001", "Test Product", 100.00m, "Test Brand", "Test Category");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Get quando o produto existe
    /// Deve retornar OK (200) com os dados do produto
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_Ok_When_Product_Exists()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        var expectedProduct = new OutputProduct
        {
            Id = 1,
            Code = "PROD001",
            Description = "Test Product"
        };

        // Configura o mock para retornar um produto quando o método Get for chamado
        _mockService.Setup(x => x.Get(productId)).Returns(expectedProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(productId);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedProduct);
    }

    /// <summary>
    /// Testa o endpoint Get quando o produto não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        OutputProduct? nullProduct = null;
        // Configura o mock para retornar null quando o método Get for chamado
        _mockService.Setup(x => x.Get(productId)).Returns(nullProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(productId);

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
        var productId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método Get for chamado
        _mockService.Setup(x => x.Get(productId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Get(productId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Update quando o produto é atualizado com sucesso
    /// Deve retornar OK (200) com os dados do produto atualizado
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_Ok_When_Product_Is_Updated_Successfully()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        var inputUpdate = new InputUpdateProduct("Updated Product", 150.00m, "Updated Brand", "Updated Category");

        var expectedProduct = new OutputProduct
        {
            Id = 1,
            Code = "PROD001",
            Description = "Updated Product",
            Price = 150.00m
        };

        // Configura o mock para retornar o produto atualizado quando o método Update for chamado
        _mockService.Setup(x => x.Update(productId, inputUpdate)).Returns(expectedProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.Update(productId, inputUpdate);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedProduct);
    }

    /// <summary>
    /// Testa o endpoint Update quando o produto não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var productId = 1L;
        var inputUpdate = new InputUpdateProduct("Updated Product", 150.00m, "Updated Brand", "Updated Category");

        // Configura o mock para lançar KeyNotFoundException quando o método Update for chamado
        _mockService.Setup(x => x.Update(productId, inputUpdate)).Throws(new KeyNotFoundException("Id inválido ou inexistente."));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(productId, inputUpdate);

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
        var productId = 1L;
        var inputUpdate = new InputUpdateProduct("Updated Product", 150.00m, "Updated Brand", "Updated Category");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Update for chamado
        _mockService.Setup(x => x.Update(productId, inputUpdate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(productId, inputUpdate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há produtos cadastrados
    /// Deve retornar OK (200) com a lista paginada de produtos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Products_Exist()
    {
        // Arrange - Preparação do teste
        var pageNumber = 1;
        var pageSize = 10;
        var products = new List<OutputProduct>
        {
            new OutputProduct { Id = 1, Code = "PROD001", Description = "Product 1" },
            new OutputProduct { Id = 2, Code = "PROD002", Description = "Product 2" }
        };

        var paginatedResult = new PaginatedResult<OutputProduct>
        {
            Items = products,
            TotalItems = 2,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };

        var filter = new InputFilterProduct();
        // Configura o mock para retornar a lista paginada quando o método GetAllByFilter for chamado
        _mockService.Setup(x => x.GetAllByFilter(filter, pageNumber, pageSize)).Returns(paginatedResult);

        // Act - Execução do método a ser testado
        var result = await _controller.GetAllByFilter(filter, pageNumber, pageSize);

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputProduct>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(products?.Count ?? 0, response.Result.Items?.Count() ?? 0);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterProduct();
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

    #region GetTopByOrders Tests

    /// <summary>
    /// Testa o endpoint GetTopByOrders quando há um produto com mais pedidos
    /// Deve retornar OK (200) com o produto que mais está agregado a pedidos
    /// </summary>
    [Fact]
    public async Task GetTopByOrders_Should_Return_Ok_With_Top_Product()
    {
        // Arrange - Preparação do teste
        var topProduct = new OutputProduct
        {
            Id = 1,
            Code = "PROD001",
            Description = "Product Description"
        };
        _mockService.Setup(x => x.GetTopByOrders()).Returns(topProduct);

        // Act - Execução do método a ser testado
        var result = await _controller.GetTopByOrders();

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(200, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<OutputProduct>;
        Assert.NotNull(response);
        Assert.NotNull(response.Result);
        Assert.Equal("PROD001", response.Result.Code);
    }

    /// <summary>
    /// Testa o endpoint GetTopByOrders quando não há produtos
    /// Deve retornar NotFound (404) com mensagem apropriada
    /// </summary>
    [Fact]
    public async Task GetTopByOrders_Should_Return_NotFound_When_No_Products()
    {
        // Arrange - Preparação do teste
        _mockService.Setup(x => x.GetTopByOrders()).Returns((OutputProduct?)null);

        // Act - Execução do método a ser testado
        var result = await _controller.GetTopByOrders();

        // Assert - Verificação do resultado
        var objectResult = result.Result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(404, objectResult!.StatusCode);

        var response = objectResult.Value as BaseResponseApi<string>;
        Assert.NotNull(response);
        Assert.Equal("Nenhum produto encontrado.", response.ErrorMessage);
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
        var expectedMetrics = new OutputMetricsProduct
        {
            TotalProducts = 500,
            AveragePrice = 150.50m
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
    /// Testa o endpoint GetAllByFilter quando há filtro por Code
    /// Deve retornar OK (200) com a lista filtrada de produtos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_By_Code()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterProduct { Code = "PROD" };
        var pageNumber = 1;
        var pageSize = 10;

        var products = new List<OutputProduct>
        {
            new() { Id = 1, Code = "PROD001", Description = "Product 1" },
            new() { Id = 2, Code = "PROD002", Description = "Product 2" }
        };

        var paginatedResult = new PaginatedResult<OutputProduct>
        {
            Items = products,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputProduct>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(2, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, product => Assert.Contains("PROD", product.Code));
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por Price ascendente
    /// Deve retornar OK (200) com a lista ordenada de produtos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_Price_Ascending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterProduct
        {
            OrderBy = EnumOrderByProduct.Price,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        // Criando produtos em ordem de preço ascendente para simular o resultado esperado
        var products = new List<OutputProduct>
        {
            new() { Id = 1, Code = "PROD001", Description = "Product 1", Price = 30.00m },
            new() { Id = 2, Code = "PROD002", Description = "Product 2", Price = 50.00m },
            new() { Id = 3, Code = "PROD003", Description = "Product 3", Price = 70.00m }
        };

        var paginatedResult = new PaginatedResult<OutputProduct>
        {
            Items = products,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputProduct>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);

        // Verifica se os itens estão ordenados corretamente (preço ascendente)
        var items = response.Result.Items!.ToList();
        Assert.Equal(30.00m, items[0].Price);
        Assert.Equal(50.00m, items[1].Price);
        Assert.Equal(70.00m, items[2].Price);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há ordenação por Description descendente
    /// Deve retornar OK (200) com a lista ordenada de produtos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Ordering_By_Description_Descending()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterProduct
        {
            OrderBy = EnumOrderByProduct.Description,
            OrderByDescending = true
        };
        var pageNumber = 1;
        var pageSize = 10;

        // Criando produtos em ordem alfabética descendente para simular o resultado esperado
        var products = new List<OutputProduct>
        {
            new() { Id = 1, Code = "PROD001", Description = "Gamma Product", Price = 99.99m },
            new() { Id = 2, Code = "PROD002", Description = "Beta Product", Price = 149.99m },
            new() { Id = 3, Code = "PROD003", Description = "Alpha Product", Price = 199.99m }
        };

        var paginatedResult = new PaginatedResult<OutputProduct>
        {
            Items = products,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputProduct>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(3, response.Result.Items?.Count() ?? 0);

        // Verifica se os itens estão ordenados corretamente (descrição descendente)
        var items = response.Result.Items!.ToList();
        Assert.Equal("Gamma Product", items[0].Description);
        Assert.Equal("Beta Product", items[1].Description);
        Assert.Equal("Alpha Product", items[2].Description);
    }

    /// <summary>
    /// Testa o endpoint GetAllByFilter quando há filtro combinado e ordenação
    /// Deve retornar OK (200) com a lista filtrada e ordenada de produtos
    /// </summary>
    [Fact]
    public async Task GetAllByFilter_Should_Return_Ok_When_Filtering_And_Ordering_Combined()
    {
        // Arrange - Preparação do teste
        var filter = new InputFilterProduct
        {
            Category = "Electronics",
            OrderBy = EnumOrderByProduct.Price,
            OrderByDescending = false
        };
        var pageNumber = 1;
        var pageSize = 10;

        var products = new List<OutputProduct>
        {
            new() { Id = 1, Code = "PROD001", Description = "Mouse", Category = "Electronics", Price = 25.00m },
            new() { Id = 2, Code = "PROD002", Description = "Laptop", Category = "Electronics", Price = 1500.00m }
        };

        var paginatedResult = new PaginatedResult<OutputProduct>
        {
            Items = products,
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

        var response = objectResult.Value as BaseResponseApi<PaginatedResult<OutputProduct>>;
        Assert.NotNull(response);
        Assert.NotNull(response!.Result);
        Assert.Equal(2, response.Result.Items?.Count() ?? 0);
        Assert.All(response.Result.Items!, product => Assert.Equal("Electronics", product.Category));

        // Verifica se os itens estão ordenados corretamente por preço ascendente
        var items = response.Result.Items!.ToList();
        Assert.Equal(25.00m, items[0].Price);
        Assert.Equal(1500.00m, items[1].Price);
    }

    #endregion
}