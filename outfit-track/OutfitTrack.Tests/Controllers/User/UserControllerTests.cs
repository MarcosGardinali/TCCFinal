using Moq;
using OutfitTrack.Api.Controllers;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Arguments;

namespace OutfitTrack.Tests.Controllers;

/// <summary>
/// Testes unitários para o UserController
/// </summary>
public class UserControllerTests : BaseControllerTests
{
    // Mock do serviço de usuário - usado para simular o comportamento da camada de negócio
    private readonly Mock<IUserService> _mockService;
    // Instância do controlador que será testado
    private readonly UserController _controller;

    /// <summary>
    /// Construtor que configura os mocks e instancia o controlador
    /// </summary>
    public UserControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UserController(MockApiDataService.Object, _mockService.Object);
    }

    /// <summary>
    /// Testa o endpoint Create quando o usuário é criado com sucesso
    /// Deve retornar Created (201) com os dados do usuário criado
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_Created_When_User_Is_Created_Successfully()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateUser("test@example.com", "Password123!", "Password123!");

        var expectedUser = new OutputUser
        {
            Id = 1,
            Email = "test@example.com"
        };

        // Configura o mock para retornar o usuário criado quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Returns(expectedUser);

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertCreatedResult(result, expectedUser);
    }

    /// <summary>
    /// Testa o endpoint Create quando o e-mail já está cadastrado
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Email_Already_Exists()
    {
        // Arrange - Preparação do teste
        var inputCreate = new InputCreateUser("test@example.com", "Password123!", "Password123!");

        var exceptionMessage = "E-mail 'test@example.com' já cadastrado.";
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
        var inputCreate = new InputCreateUser("test@example.com", "Password123!", "Password123!");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Create for chamado
        _mockService.Setup(x => x.Create(inputCreate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Create(inputCreate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint GetByEmail quando o usuário existe
    /// Deve retornar OK (200) com os dados do usuário
    /// </summary>
    [Fact]
    public async Task GetByEmail_Should_Return_Ok_When_User_Exists()
    {
        // Arrange - Preparação do teste
        var email = "test@example.com";
        var expectedUser = new OutputUser
        {
            Id = 1,
            Email = email
        };

        // Configura o mock para retornar um usuário quando o método GetByEmail for chamado
        _mockService.Setup(x => x.GetByEmail(email)).Returns(expectedUser);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByEmail(email);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedUser);
    }

    /// <summary>
    /// Testa o endpoint GetByEmail quando o usuário não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task GetByEmail_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var email = "test@example.com";
        OutputUser? nullUser = null;
        // Configura o mock para retornar null quando o método GetByEmail for chamado
        _mockService.Setup(x => x.GetByEmail(email)).Returns(nullUser);

        // Act - Execução do método a ser testado
        var result = await _controller.GetByEmail(email);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Item não encontrado.");
    }

    /// <summary>
    /// Testa o endpoint GetByEmail quando ocorre uma exceção
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task GetByEmail_Should_Return_BadRequest_When_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var email = "test@example.com";
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método GetByEmail for chamado
        _mockService.Setup(x => x.GetByEmail(email)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.GetByEmail(email);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint RedefinePassword quando a senha é redefinida com sucesso
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task RedefinePassword_Should_Return_Ok_When_Password_Is_Redefined_Successfully()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputRedefinePassword = new InputRedefinePasswordUser("oldpassword", "NewPassword123!", "NewPassword123!");

        // Configura o mock para retornar true quando o método RedefinePassword for chamado
        _mockService.Setup(x => x.RedefinePassword(userId, inputRedefinePassword)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.RedefinePassword(userId, inputRedefinePassword);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint RedefinePassword quando o usuário não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task RedefinePassword_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputRedefinePassword = new InputRedefinePasswordUser("oldpassword", "NewPassword123!", "NewPassword123!");

        // Configura o mock para lançar KeyNotFoundException quando o método RedefinePassword for chamado
        _mockService.Setup(x => x.RedefinePassword(userId, inputRedefinePassword)).Throws(new KeyNotFoundException("Id inválido ou inexistente."));

        // Act - Execução do método a ser testado
        var result = await _controller.RedefinePassword(userId, inputRedefinePassword);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Id inválido ou inexistente.");
    }

    /// <summary>
    /// Testa o endpoint RedefinePassword quando a senha antiga está incorreta
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task RedefinePassword_Should_Return_BadRequest_When_Old_Password_Is_Incorrect()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputRedefinePassword = new InputRedefinePasswordUser("oldpassword", "NewPassword123!", "NewPassword123!");

        var exceptionMessage = "Senha antiga incorreta.";
        // Configura o mock para lançar InvalidOperationException quando o método RedefinePassword for chamado
        _mockService.Setup(x => x.RedefinePassword(userId, inputRedefinePassword)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.RedefinePassword(userId, inputRedefinePassword);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint RedefinePassword quando ocorre uma exceção genérica
    /// Deve retornar BadRequest (400) com a mensagem da exceção
    /// </summary>
    [Fact]
    public async Task RedefinePassword_Should_Return_BadRequest_When_Generic_Exception_Occurs()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputRedefinePassword = new InputRedefinePasswordUser("oldpassword", "NewPassword123!", "NewPassword123!");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método RedefinePassword for chamado
        _mockService.Setup(x => x.RedefinePassword(userId, inputRedefinePassword)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.RedefinePassword(userId, inputRedefinePassword);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Get quando o usuário existe
    /// Deve retornar OK (200) com os dados do usuário
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_Ok_When_User_Exists()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var expectedUser = new OutputUser
        {
            Id = 1,
            Email = "test@example.com"
        };

        // Configura o mock para retornar um usuário quando o método Get for chamado
        _mockService.Setup(x => x.Get(userId)).Returns(expectedUser);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(userId);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedUser);
    }

    /// <summary>
    /// Testa o endpoint Get quando o usuário não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Get_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        OutputUser? nullUser = null;
        // Configura o mock para retornar null quando o método Get for chamado
        _mockService.Setup(x => x.Get(userId)).Returns(nullUser);

        // Act - Execução do método a ser testado
        var result = await _controller.Get(userId);

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
        var userId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção quando o método Get for chamado
        _mockService.Setup(x => x.Get(userId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Get(userId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Update quando o usuário é atualizado com sucesso
    /// Deve retornar OK (200) com os dados do usuário atualizado
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_Ok_When_User_Is_Updated_Successfully()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputUpdate = new InputUpdateUser("newemail@example.com", "NewPassword123!");

        var expectedUser = new OutputUser
        {
            Id = 1,
            Email = "newemail@example.com"
        };

        // Configura o mock para retornar o usuário atualizado quando o método Update for chamado
        _mockService.Setup(x => x.Update(userId, inputUpdate)).Returns(expectedUser);

        // Act - Execução do método a ser testado
        var result = await _controller.Update(userId, inputUpdate);

        // Assert - Verificação do resultado
        AssertOkResult(result, expectedUser);
    }

    /// <summary>
    /// Testa o endpoint Update quando o usuário não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputUpdate = new InputUpdateUser("newemail@example.com", "NewPassword123!");

        // Configura o mock para lançar KeyNotFoundException quando o método Update for chamado
        _mockService.Setup(x => x.Update(userId, inputUpdate)).Throws(new KeyNotFoundException("Id inválido ou inexistente."));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(userId, inputUpdate);

        // Assert - Verificação do resultado
        AssertNotFoundResult(result, "Id inválido ou inexistente.");
    }

    /// <summary>
    /// Testa o endpoint Update quando a senha está incorreta
    /// Deve retornar BadRequest (400) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Password_Is_Incorrect()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        var inputUpdate = new InputUpdateUser("newemail@example.com", "NewPassword123!");

        var exceptionMessage = "Senha incorreta.";
        // Configura o mock para lançar InvalidOperationException quando o método Update for chamado
        _mockService.Setup(x => x.Update(userId, inputUpdate)).Throws(new InvalidOperationException(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(userId, inputUpdate);

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
        var userId = 1L;
        var inputUpdate = new InputUpdateUser("newemail@example.com", "NewPassword123!");

        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Update for chamado
        _mockService.Setup(x => x.Update(userId, inputUpdate)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Update(userId, inputUpdate);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o usuário existe e pode ser excluído
    /// Deve retornar OK (200) com resultado true
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_Ok_When_User_Can_Be_Deleted()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        // Configura o mock para retornar true quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(userId)).Returns(true);

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(userId);

        // Assert - Verificação do resultado
        AssertOkResult(result, true);
    }

    /// <summary>
    /// Testa o endpoint Delete quando o usuário não existe
    /// Deve retornar NotFound (404) com mensagem de erro
    /// </summary>
    [Fact]
    public async Task Delete_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        // Arrange - Preparação do teste
        var userId = 1L;
        // Configura o mock para lançar KeyNotFoundException quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(userId)).Throws(new KeyNotFoundException("Item não encontrado para exclusão."));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(userId);

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
        var userId = 1L;
        var exceptionMessage = "Database error";
        // Configura o mock para lançar uma exceção genérica quando o método Delete for chamado
        _mockService.Setup(x => x.Delete(userId)).Throws(new Exception(exceptionMessage));

        // Act - Execução do método a ser testado
        var result = await _controller.Delete(userId);

        // Assert - Verificação do resultado
        AssertBadRequestResult(result, exceptionMessage);
    }
}