using Microsoft.AspNetCore.Http;
using Moq;
using OutfitTrack.Application.Interfaces;
using OutfitTrack.Application.Security;
using OutfitTrack.Application.Services;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Testes unitários para o AuthenticationService
/// Testa todas as validações e cenários de exceção do serviço
/// </summary>
public class AuthenticationServiceTests
{
    #region Authenticate Method Tests
    /// <summary>
    /// Testa que o método Authenticate lança InvalidOperationException quando o usuário não existe
    /// </summary>
    [Fact]
    public void Authenticate_Should_Throw_InvalidOperationException_When_User_Does_Not_Exist()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserService = new Mock<IUserService>();

        // Configura o HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Host = new HostString("localhost");
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var service = new AuthenticationService(mockHttpContextAccessor.Object, mockUserRepository.Object, mockUserService.Object);

        var inputAuthentication = new InputAuthentication("nonexistent@example.com", "Password123!");

        // Usuário não existe
        mockUserRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Authenticate(inputAuthentication));
        Assert.Equal("Usuário não existe. Cadastre seu usuário no endpoint aberto POST '/api/User'", exception.Message);
    }

    /// <summary>
    /// Testa que o método Authenticate lança InvalidOperationException quando a senha está incorreta
    /// </summary>
    [Fact]
    public void Authenticate_Should_Throw_InvalidOperationException_When_Password_Is_Incorrect()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserService = new Mock<IUserService>();

        // Configura o HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Host = new HostString("localhost");
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var service = new AuthenticationService(mockHttpContextAccessor.Object, mockUserRepository.Object, mockUserService.Object);

        var inputAuthentication = new InputAuthentication("john.doe@example.com", "WrongPassword123!");

        // Usuário existe
        var existingUser = new User("john.doe@example.com", PasswordEncryption.Encrypt("Password123!"), null);
        existingUser.SetProperty(nameof(User.Id), 1L);
        mockUserRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Authenticate(inputAuthentication));
        Assert.Equal("Usuário não autorizado. Senha incorreta.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Authenticate retorna um token com sucesso quando as credenciais estão corretas
    /// </summary>
    [Fact]
    public void Authenticate_Should_Return_Token_When_Credentials_Are_Correct()
    {
        // Arrange
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserService = new Mock<IUserService>();

        // Configura o HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Host = new HostString("localhost");
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var service = new AuthenticationService(mockHttpContextAccessor.Object, mockUserRepository.Object, mockUserService.Object);

        var inputAuthentication = new InputAuthentication("john.doe@example.com", "Password123!");

        // Usuário existe
        var existingUser = new User("john.doe@example.com", PasswordEncryption.Encrypt("Password123!"), null);
        existingUser.SetProperty(nameof(User.Id), 1L);
        mockUserRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Configura o UserService para atualizar a data de expiração do token
        mockUserService.Setup(s => s.UpdateTokenExpirationDate(It.IsAny<long>()))
            .Verifiable();

        // Act
        var result = service.Authenticate(inputAuthentication);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.TokenExpirationDate);
        mockUserService.Verify(s => s.UpdateTokenExpirationDate(1L), Times.Once);
    }
    #endregion
}