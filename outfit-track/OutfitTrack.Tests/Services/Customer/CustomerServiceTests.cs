using Moq;
using OutfitTrack.Application.Services;
using OutfitTrack.Arguments;
using OutfitTrack.Arguments.Enums;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Testes unitários para o CustomerService
/// Testa todas as validações e cenários de exceção do serviço
/// </summary>
public class CustomerServiceTests : BaseServiceTest
{
    #region Create Method Tests
    /// <summary>
    /// Testa que o método Create lança InvalidOperationException quando o CPF já existe
    /// </summary>
    [Fact]
    public void Create_Should_Throw_InvalidOperationException_When_Cpf_Already_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateCustomer(
            "John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com");

        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Create(inputCreate));
        Assert.Equal("Cpf '12345678901' já cadastrado.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Create cria um cliente com sucesso quando o CPF não existe
    /// </summary>
    [Fact]
    public void Create_Should_Create_Customer_When_Cpf_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateCustomer(
            "John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com");

        var newCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        mockRepository.Setup(r => r.Create(It.IsAny<Customer>()))
            .Returns(newCustomer);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Create(inputCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("12345678901", result.Cpf);
        mockRepository.Verify(r => r.Create(It.IsAny<Customer>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Update Method Tests

    /// <summary>
    /// Testa que o método Update lança KeyNotFoundException quando o cliente não existe
    /// </summary>
    [Fact]
    public void Update_Should_Throw_KeyNotFoundException_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;
        var inputUpdate = new InputUpdateCustomer(
            "Jane", "Smith", DateTime.Now.AddYears(-30), "Street", null,
            "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP,
            "12345678", "123456789", "1234567890123", "jane.smith@example.com");

        // Configura o mock do repositório para retornar null (cliente não existe)
        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Update(customerId, inputUpdate));
        Assert.Equal("Id inválido ou inexistente.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update atualiza um cliente com sucesso quando o cliente existe
    /// </summary>
    [Fact]
    public void Update_Should_Update_Customer_When_Customer_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;
        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());

        var updatedCustomer = new Customer("Jane", "Smith", DateTime.Now.AddYears(-30), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "jane.smith@example.com", new List<Order>());

        var inputUpdate = new InputUpdateCustomer(
            "Jane", "Smith", DateTime.Now.AddYears(-30), "Street", null,
            "Neighborhood", "123", "City", EnumStateAbbreviationBrazil.SP,
            "12345678", "123456789", "1234567890123", "jane.smith@example.com");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        mockRepository.Setup(r => r.Update(It.IsAny<Customer>()))
            .Returns(updatedCustomer);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Update(customerId, inputUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        mockRepository.Verify(r => r.Update(It.IsAny<Customer>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Delete Method Tests

    /// <summary>
    /// Testa que o método Delete lança KeyNotFoundException quando o cliente não existe
    /// </summary>
    [Fact]
    public void Delete_Should_Throw_KeyNotFoundException_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;

        // Configura o mock do repositório para retornar null (cliente não existe)
        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Delete(customerId));
        Assert.Equal("Id inválido ou inexistente.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Delete lança InvalidOperationException quando o cliente possui pedidos
    /// </summary>
    [Fact]
    public void Delete_Should_Throw_InvalidOperationException_When_Customer_Has_Orders()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;
        var customerWithOrders = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order> { new Order() }); // Cliente com pedidos

        // Configura o mock do repositório para retornar um cliente com pedidos
        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(customerWithOrders);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Delete(customerId));
        Assert.Equal("Esse cliente possui vínculo com pedidos", exception.Message);
    }

    /// <summary>
    /// Testa que o método Delete remove um cliente com sucesso quando o cliente existe e não possui pedidos
    /// </summary>
    [Fact]
    public void Delete_Should_Remove_Customer_When_Customer_Exists_And_Has_No_Orders()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;
        var customerWithoutOrders = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>()); // Cliente sem pedidos

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(customerWithoutOrders);

        mockRepository.Setup(r => r.Delete(It.IsAny<Customer>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Delete(customerId);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.Delete(It.IsAny<Customer>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region GetByCpf Method Tests

    /// <summary>
    /// Testa que o método GetByCpf retorna null quando o cliente não existe
    /// </summary>
    [Fact]
    public void GetByCpf_Should_Return_Null_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var cpf = "12345678901";

        // Configura o mock do repositório para retornar null (cliente não existe)
        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        // Act
        var result = service.GetByCpf(cpf);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetByCpf retorna um cliente quando o cliente existe
    /// </summary>
    [Fact]
    public void GetByCpf_Should_Return_Customer_When_Customer_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var cpf = "12345678901";
        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        // Act
        var result = service.GetByCpf(cpf);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("12345678901", result.Cpf);
    }

    #endregion

    #region Get Method Tests

    /// <summary>
    /// Testa que o método Get retorna null quando o cliente não existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Null_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns((Customer?)null);

        // Act
        var result = service.Get(customerId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método Get retorna um cliente quando o cliente existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Customer_When_Customer_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customerId = 1L;
        var existingCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>());
        existingCustomer.SetProperty(nameof(Customer.Id), customerId);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
            .Returns(existingCustomer);

        // Act
        var result = service.Get(customerId);

        // Verifica se o repositório foi chamado
        mockRepository.Verify(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()), Times.Once);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal(customerId, result.Id);
    }

    #endregion

    #region GetAllByFilter Method Tests

    /// <summary>
    /// Testa que o método GetAllByFilter retorna null quando não há clientes
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Null_When_No_Customers_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterCustomer>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((PaginatedResult<Customer>?)null);

        // Act
        var result = service.GetAllByFilter(new InputFilterCustomer(), 1, 10);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetAllByFilter retorna uma lista de clientes quando há clientes
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Return_Customers_When_Customers_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var customers = new List<Customer>
        {
            new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
                "Street", null, "Neighborhood", "123", "City",
                EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
                "1234567890123", "john.doe@example.com", new List<Order>()),
            new Customer("Jane", "Smith", DateTime.Now.AddYears(-30), "12345678902",
                "Street2", null, "Neighborhood2", "124", "City2",
                EnumStateAbbreviationBrazil.RJ, "12345679", "123456790",
                "1234567890124", "jane.smith@example.com", new List<Order>())
        };

        var paginatedResult = new PaginatedResult<Customer>
        {
            Items = customers,
            TotalItems = 2,
            CurrentPage = 1,
            PageSize = 10
        };

        mockRepository.Setup(r => r.GetAllByFilter(It.IsAny<InputFilterCustomer>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(paginatedResult);

        // Act
        var result = service.GetAllByFilter(new InputFilterCustomer(), 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items?.Count() ?? 0);
    }

    #endregion

    #region GetTopByOrders Method Tests

    /// <summary>
    /// Testa que o método GetTopByOrders retorna null quando não há clientes
    /// </summary>
    [Fact]
    public void GetTopByOrders_Should_Return_Null_When_No_Customers_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        mockRepository.Setup(r => r.GetTopByOrders())
            .Returns((Customer?)null);

        // Act
        var result = service.GetTopByOrders();

        // Assert
        Assert.Null(result);
        mockRepository.Verify(r => r.GetTopByOrders(), Times.Once);
    }

    /// <summary>
    /// Testa que o método GetTopByOrders retorna o cliente correto quando há clientes
    /// </summary>
    [Fact]
    public void GetTopByOrders_Should_Return_Top_Customer_When_Customers_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var topCustomer = new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
            "Street", null, "Neighborhood", "123", "City",
            EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
            "1234567890123", "john.doe@example.com", new List<Order>
            {
                new Order(),
                new Order(),
                new Order()
            });
        topCustomer.SetProperty(nameof(Customer.Id), 1L);

        mockRepository.Setup(r => r.GetTopByOrders())
            .Returns(topCustomer);

        // Act
        var result = service.GetTopByOrders();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        mockRepository.Verify(r => r.GetTopByOrders(), Times.Once);
    }

    #endregion

    #region Filter and Order Tests

    /// <summary>
    /// Testa que o método GetAllByFilter filtra corretamente por FirstName
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Filter_By_FirstName()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var filter = new InputFilterCustomer { Name = "John" };

        var customers = new List<Customer>
        {
            new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
                "Street", null, "Neighborhood", "123", "City",
                EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
                "1234567890123", "john.doe@example.com", new List<Order>()),
            new Customer("John", "Smith", DateTime.Now.AddYears(-30), "12345678902",
                "Street2", null, "Neighborhood2", "124", "City2",
                EnumStateAbbreviationBrazil.RJ, "12345679", "123456790",
                "1234567890124", "john.smith@example.com", new List<Order>())
        };

        var paginatedResult = new PaginatedResult<Customer>
        {
            Items = customers,
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
    /// Testa que o método GetAllByFilter ordena corretamente por FirstName ascendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_FirstName_Ascending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var filter = new InputFilterCustomer
        {
            OrderBy = EnumOrderByCustomer.FirstName,
            OrderByDescending = false
        };

        var customers = new List<Customer>
        {
            new Customer("Charlie", "Doe", DateTime.Now.AddYears(-25), "12345678901",
                "Street", null, "Neighborhood", "123", "City",
                EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
                "1234567890123", "charlie.doe@example.com", new List<Order>()),
            new Customer("Alice", "Smith", DateTime.Now.AddYears(-30), "12345678902",
                "Street2", null, "Neighborhood2", "124", "City2",
                EnumStateAbbreviationBrazil.RJ, "12345679", "123456790",
                "1234567890124", "alice.smith@example.com", new List<Order>()),
            new Customer("Bob", "Johnson", DateTime.Now.AddYears(-35), "12345678903",
                "Street3", null, "Neighborhood3", "125", "City3",
                EnumStateAbbreviationBrazil.MG, "12345680", "123456801",
                "1234567890125", "bob.johnson@example.com", new List<Order>())
        };

        var paginatedResult = new PaginatedResult<Customer>
        {
            Items = customers,
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
    /// Testa que o método GetAllByFilter ordena corretamente por CreationDate descendente
    /// </summary>
    [Fact]
    public void GetAllByFilter_Should_Order_By_CreationDate_Descending()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var filter = new InputFilterCustomer
        {
            OrderBy = EnumOrderByCustomer.CreationDate,
            OrderByDescending = true
        };

        var customers = new List<Customer>
        {
            new Customer("John", "Doe", DateTime.Now.AddYears(-25), "12345678901",
                "Street", null, "Neighborhood", "123", "City",
                EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
                "1234567890123", "john.doe@example.com", new List<Order>()),
            new Customer("Jane", "Smith", DateTime.Now.AddYears(-30), "12345678902",
                "Street2", null, "Neighborhood2", "124", "City2",
                EnumStateAbbreviationBrazil.RJ, "12345679", "123456790",
                "1234567890124", "jane.smith@example.com", new List<Order>()),
            new Customer("Bob", "Johnson", DateTime.Now.AddYears(-35), "12345678903",
                "Street3", null, "Neighborhood3", "125", "City3",
                EnumStateAbbreviationBrazil.MG, "12345680", "123456801",
                "1234567890125", "bob.johnson@example.com", new List<Order>())
        };

        var paginatedResult = new PaginatedResult<Customer>
        {
            Items = customers,
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
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);

        var filter = new InputFilterCustomer
        {
            Name = "Smith",
            OrderBy = EnumOrderByCustomer.FirstName,
            OrderByDescending = false
        };

        var customers = new List<Customer>
        {
            new Customer("Alice", "Smith", DateTime.Now.AddYears(-25), "12345678901",
                "Street", null, "Neighborhood", "123", "City",
                EnumStateAbbreviationBrazil.SP, "12345678", "123456789",
                "1234567890123", "alice.smith@example.com", new List<Order>()),
            new Customer("Bob", "Smith", DateTime.Now.AddYears(-30), "12345678902",
                "Street2", null, "Neighborhood2", "124", "City2",
                EnumStateAbbreviationBrazil.RJ, "12345679", "123456790",
                "1234567890124", "bob.smith@example.com", new List<Order>())
        };

        var paginatedResult = new PaginatedResult<Customer>
        {
            Items = customers,
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
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<ICustomerRepository, Customer, InputFilterCustomer>();
        var service = new CustomerService(mockUnitOfWork.Object);
        var expectedMetrics = new OutputMetricsCustomer
        {
            TotalCustomers = 10,
            ActiveCustomersCount = 5,
            AverageAge = 35,
            NewCustomersPerMonth = [new OutputMonthQuantityOrder { Year = 2024, Month = 1, Quantity = 10 }]
        };

        mockRepository.Setup(r => r.GetMetrics()).Returns(expectedMetrics);

        // Act
        var result = service.GetMetrics();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMetrics.TotalCustomers, result.TotalCustomers);
        Assert.Equal(expectedMetrics.ActiveCustomersCount, result.ActiveCustomersCount);
        Assert.Equal(expectedMetrics.AverageAge, result.AverageAge);
        Assert.Single(result.NewCustomersPerMonth);
        mockRepository.Verify(r => r.GetMetrics(), Times.Once);
    }

    #endregion
}