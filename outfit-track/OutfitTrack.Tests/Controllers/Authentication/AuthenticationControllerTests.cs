using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Tests.Controllers;

/// <summary>
/// Testes unitários para o AuthenticationController
/// </summary>
public class AuthenticationControllerTests : BaseControllerTests
{
    // Mock do serviço de autenticação - usado para simular o comportamento da camada de negócio
    private readonly Mock<IAuthenticationService> _mockService;
    // Instância do controlador que será testado
    private readonly AuthenticationController _controller;

    /// <summary>
    /// Construtor que configura os mocks e instancia o controlador
    /// </summary>
    public AuthenticationControllerTests()
    {
        _mockService = new Mock<IAuthenticationService>();
        _controller = new AuthenticationController(MockApiDataService.Object, _mockService.Object);
    }

    /// <summary>
    /// Testa o endpoint Authenticate quando as credenciais são válidas
    /// Deve retornar OK (200) com os dados de autenticação
    /// </summary>
    [Fact]
    public async Task Authenticate_Should_Return_Ok_When_Credentials_Are_Valid()
    {
        // Arrange - Preparação do teste
        var inputAuthentication = new InputAuthentication("test@example.com", "Password123!");

        var expectedAuthentication = new OutputAuthentication("valid_token", DateTime.UtcNow.AddDays(7));

        // Configura o mock para retornar os dados de autenticação quando o método Authenticate for chamado
        _mockService.Setup(x => x.Authenticate(inputAuthentication)).Returns(expectedAuthentication);

        // Act - Execução do método a ser testado
        var result = await _controller.Authenticate(inputAuthentication);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedAuthentication);
    }

    /// <summary>
    /// Testa o endpoint Authenticate quando o usuário não existe
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Authenticate_Should_Return_BadRequest_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var inputAuthentication = new InputAuthentication("nonexistent@example.com", "Password123!");

        var exceptionMessage = "Usuário não existe. Cadastre seu usuário no endpoint aberto POST '/api/User'";
        // Configura o mock para lançar InvalidOperationException quando o método Authenticate for chamado
        _mockService.Setup(x => x.Authenticate(inputAuthentication)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Authenticate(inputAuthentication);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Authenticate quando a senha está incorreta
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Authenticate_Should_Return_BadRequest_When_Password_Is_Incorrect()
    {
        // Arrange - Preparação do teste
        var inputAuthentication = new InputAuthentication("test@example.com", "WrongPassword123!");

        var exceptionMessage = "Usuário não autorizado. Senha incorreta.";
        // Configura o mock para lançar InvalidOperationException quando o método Authenticate for chamado
        _mockService.Setup(x => x.Authenticate(inputAuthentication)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Authenticate(inputAuthentication);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Authenticate quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task Authenticate_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var inputAuthentication = new InputAuthentication("test@example.com", "Password123!");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Authenticate for chamado
        _mockService.Setup(x => x.Authenticate(inputAuthentication)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Authenticate(inputAuthentication);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }
}