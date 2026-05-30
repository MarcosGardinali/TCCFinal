using Moq;
using OutfitTrack.Application.Services;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Testes unitários para o ProductService
/// Testa todas as validações e cenários de exceção do serviço
/// </summary>
public class ProductServiceTests : BaseServiceTest
{
    #region Create Method Tests

    /// <summary>
    /// Testa que o método Create lança InvalidOperationException quando o código já existe
    /// </summary>
    [Fact]
    public void Create_Should_Throw_InvalidOperationException_When_Code_Already_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateProduct("PROD001", "Product Description", 99.99m, "Brand", "Category");

        var existingProduct = new Product("PROD001", "Existing Product", 59.99m, "Existing Brand", "Existing Category", new List<OrderItem>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Create(inputCreate));
        Assert.Equal("Código 'PROD001' já cadastrado.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Create cria um produto com sucesso quando o código não existe
    /// </summary>
    [Fact]
    public void Create_Should_Create_Product_When_Code_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateProduct("PROD001", "Product Description", 99.99m, "Brand", "Category");

        var newProduct = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>());
        newProduct.SetProperty(nameof(Product.Id), 1L);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        mockRepository.Setup(r => r.Create(It.IsAny<Product>()))
            .Returns(newProduct);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Create(inputCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD001", result.Code);
        Assert.Equal("Product Description", result.Description);
        mockRepository.Verify(r => r.Create(It.IsAny<Product>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Update Method Tests

    /// <summary>
    /// Testa que o método Update lança KeyNotFoundException quando o produto não existe
    /// </summary>
    [Fact]
    public void Update_Should_Throw_KeyNotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;
        var inputUpdate = new InputUpdateProduct("Updated Description", 149.99m, "Updated Brand", "Updated Category");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Update(productId, inputUpdate));
        Assert.Equal("Id inválido ou inexistente.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update atualiza um produto com sucesso quando o produto existe
    /// </summary>
    [Fact]
    public void Update_Should_Update_Product_When_Product_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;
        var existingProduct = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>());
        existingProduct.SetProperty(nameof(Product.Id), productId);

        var updatedProduct = new Product("PROD001", "Updated Description", 149.99m, "Updated Brand", "Updated Category", new List<OrderItem>());
        updatedProduct.SetProperty(nameof(Product.Id), productId);

        var inputUpdate = new InputUpdateProduct("Updated Description", 149.99m, "Updated Brand", "Updated Category");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct);

        mockRepository.Setup(r => r.Update(It.IsAny<Product>()))
            .Returns(updatedProduct);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Update(productId, inputUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(149.99m, result.Price);
        mockRepository.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Delete Method Tests

    /// <summary>
    /// Testa que o método Delete lança KeyNotFoundException quando o produto não existe
    /// </summary>
    [Fact]
    public void Delete_Should_Throw_KeyNotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Delete(productId));
        Assert.Equal("Id inválido ou inexistente.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Delete lança InvalidOperationException quando o produto possui itens de pedido
    /// </summary>
    [Fact]
    public void Delete_Should_Throw_InvalidOperationException_When_Product_Has_Order_Items()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;
        var productWithOrderItems = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem> { new OrderItem() });
        productWithOrderItems.SetProperty(nameof(Product.Id), productId);

        // Produto com itens de pedido
        productWithOrderItems.SetProperty(nameof(Product.ListOrderItem), new List<OrderItem> { new OrderItem() });

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(productWithOrderItems);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Delete(productId));
        Assert.Equal("Esse produto possui vínculo com itens de pedido", exception.Message);
    }

    /// <summary>
    /// Testa que o método Delete remove um produto com sucesso quando o produto existe e não possui itens de pedido
    /// </summary>
    [Fact]
    public void Delete_Should_Remove_Product_When_Product_Exists_And_Has_No_Order_Items()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;
        var productWithoutOrderItems = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>());
        productWithoutOrderItems.SetProperty(nameof(Product.Id), productId);

        // Produto sem itens de pedido
        productWithoutOrderItems.SetProperty(nameof(Product.ListOrderItem), new List<OrderItem>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(productWithoutOrderItems);

        mockRepository.Setup(r => r.Delete(It.IsAny<Product>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Delete(productId);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region GetByCode Method Tests

    /// <summary>
    /// Testa que o método GetByCode retorna null quando o produto não existe
    /// </summary>
    [Fact]
    public void GetByCode_Should_Return_Null_When_Product_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var code = "NONEXISTENT";

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        // Act
        var result = service.GetByCode(code);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetByCode retorna um produto quando o produto existe
    /// </summary>
    [Fact]
    public void GetByCode_Should_Return_Product_When_Product_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var code = "PROD001";
        var existingProduct = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>());
        existingProduct.SetProperty(nameof(Product.Id), 1L);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct);

        // Act
        var result = service.GetByCode(code);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD001", result.Code);
        Assert.Equal(1L, result.Id);
    }

    #endregion

    #region Get Method Tests

    /// <summary>
    /// Testa que o método Get retorna null quando o produto não existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Null_When_Product_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        // Act
        var result = service.Get(productId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método Get retorna um produto quando o produto existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Product_When_Product_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var productId = 1L;
        var existingProduct = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>());
        existingProduct.SetProperty(nameof(Product.Id), productId);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .Returns(existingProduct);

        // Act
        var result = service.Get(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD001", result.Code);
        Assert.Equal(productId, result.Id);
    }

    #endregion

    #region GetAllByFilter Method Tests

    /// <summary>
    /// Testa que o método GetAllByFilter retorna null quando não há produtos
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Null_When_No_Products_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterProduct>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((PaginatedResult<Product>?)null);

        // Act
        var result = service.GetAllByFilter(new InputFilterProduct(), 1, 10);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter retorna uma lista de produtos quando há produtos
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Products_When_Products_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var products = new List<Product>
        {
            new Product("PROD001", "Product 1", 99.99m, "Brand 1", "Category 1", new List<OrderItem>()),
            new Product("PROD002", "Product 2", 149.99m, "Brand 2", "Category 2", new List<OrderItem>())
        };

        var paginatedResult = new PaginatedResult<Product>
        {
            Items = products,
            TotalItems = 2,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterProduct>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(new InputFilterProduct(), 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items?.Count() ?? 0);
    }

    #endregion

    #region GetTopByOrders Method Tests

    /// <summary>
    /// Testa que o método GetTopByOrders retorna null quando não há produtos
    /// </summary>
    [Fact]
    public void GetTopByOrders_Should_Return_Null_When_No_Products_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        mockRepository.Setup(r => r.GetTopByOrders())
            .Returns((Product?)null);

        // Act
        var result = service.GetTopByOrders();

        // Assert
        Assert.Null(result);
        mockRepository.Verify(r => r.GetTopByOrders(), Times.Once);
    }

    /// <summary>
    /// Testa que o método GetTopByOrders retorna o produto correto quando há produtos
    /// </summary>
    [Fact]
    public void GetTopByOrders_Should_Return_Top_Product_When_Products_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var topProduct = new Product("PROD001", "Product Description", 99.99m, "Brand", "Category", new List<OrderItem>
        {
            new OrderItem(),
            new OrderItem(),
            new OrderItem(),
            new OrderItem(),
            new OrderItem()
        });
        topProduct.SetProperty(nameof(Product.Id), 1L);

        mockRepository.Setup(r => r.GetTopByOrders())
            .Returns(topProduct);

        // Act
        var result = service.GetTopByOrders();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD001", result.Code);
        mockRepository.Verify(r => r.GetTopByOrders(), Times.Once);
    }

    #endregion

    #region Filter and Order Tests

    /// <summary>
    /// Testa que o método GetAllByFilter filtra corretamente por Code
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Filter_By_Code()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var filter = new InputFilterProduct { Code = "PROD" };

        var products = new List<Product>
        {
            new Product("PROD001", "Product 1", 99.99m, "Brand 1", "Category 1", new List<OrderItem>()),
            new Product("PROD002", "Product 2", 149.99m, "Brand 2", "Category 2", new List<OrderItem>())
        };

        var paginatedResult = new PaginatedResult<Product>
        {
            Items = products,
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
    /// Testa que o método GetAllByFilter ordena corretamente por Price ascendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_Price_Ascending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var filter = new InputFilterProduct
        {
            OrderBy = EnumOrderByProduct.Price,
            OrderByDescending = false
        };

        var products = new List<Product>
        {
            new Product("PROD001", "Product 1", 150.00m, "Brand 1", "Category 1", new List<OrderItem>()),
            new Product("PROD002", "Product 2", 50.00m, "Brand 2", "Category 2", new List<OrderItem>()),
            new Product("PROD003", "Product 3", 250.00m, "Brand 3", "Category 3", new List<OrderItem>())
        };

        var paginatedResult = new PaginatedResult<Product>
        {
            Items = products,
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
    /// Testa que o método GetAllByFilter ordena corretamente por Description descendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_Description_Descending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var filter = new InputFilterProduct
        {
            OrderBy = EnumOrderByProduct.Description,
            OrderByDescending = true
        };

        var products = new List<Product>
        {
            new Product("PROD001", "Gamma Product", 99.99m, "Brand 1", "Category 1", new List<OrderItem>()),
            new Product("PROD002", "Alpha Product", 149.99m, "Brand 2", "Category 2", new List<OrderItem>()),
            new Product("PROD003", "Beta Product", 199.99m, "Brand 3", "Category 3", new List<OrderItem>())
        };

        var paginatedResult = new PaginatedResult<Product>
        {
            Items = products,
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
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);

        var filter = new InputFilterProduct
        {
            Category = "Electronics",
            OrderBy = EnumOrderByProduct.Price,
            OrderByDescending = false
        };

        var products = new List<Product>
        {
            new Product("PROD001", "Laptop", 1500.00m, "Brand 1", "Electronics", new List<OrderItem>()),
            new Product("PROD002", "Mouse", 25.00m, "Brand 2", "Electronics", new List<OrderItem>())
        };

        var paginatedResult = new PaginatedResult<Product>
        {
            Items = products,
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

    #endregion

    #region Metrics Method Tests

    /// <summary>
    /// Testa que o método GetCompleteMetrics retorna as métricas do repositório
    /// </summary>
    [Fact]
    public void GetCompleteMetrics_Should_Return_Metrics_From_Repository()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IProductRepository, Product, InputFilterProduct>();
        var service = new ProductService(mockUnitOfWork.Object);
        var expectedMetrics = new OutputMetricsProduct
        {
            TotalProducts = 50,
            AveragePrice = 125.50m
        };

        mockRepository.Setup(r => r.GetMetrics()).Returns(expectedMetrics);

        // Act
        var result = service.GetMetrics();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMetrics.TotalProducts, result.TotalProducts);
        Assert.Equal(expectedMetrics.AveragePrice, result.AveragePrice);
        mockRepository.Verify(r => r.GetMetrics(), Times.Once);
    }

    #endregion
}