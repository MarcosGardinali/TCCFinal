using Moq;
using OutfitTrack.Application.Security;
using OutfitTrack.Application.Services;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Tests.Services;

/// <summary>
/// Testes unitários para o UserService
/// Testa todas as validações e cenários de exceção do serviço
/// </summary>
public class UserServiceTests : BaseServiceTest
{
    #region Create Method Tests
    /// <summary>
    /// Testa que o método Create lança InvalidOperationException quando o e-mail já existe
    /// </summary>
    [Fact]
    public void Create_Should_Throw_InvalidOperationException_When_Email_Already_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateUser("john.doe@example.com", "Password123!", "Password123!");

        var existingUser = new User("john.doe@example.com", "encryptedPassword", DateTime.UtcNow.AddDays(7));

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Create(inputCreate));
        Assert.Equal("E-mail 'john.doe@example.com' já cadastrado.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Create cria um usuário com sucesso quando o e-mail não existe
    /// </summary>
    [Fact]
    public void Create_Should_Create_User_When_Email_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var inputCreate = new InputCreateUser("john.doe@example.com", "Password123!", "Password123!");

        var newUser = new User("john.doe@example.com", PasswordEncryption.Encrypt("Password123!"), DateTime.UtcNow.AddDays(7));
        newUser.SetProperty(nameof(User.Id), 1L);
        newUser.SetProperty(nameof(User.TokenExpirationDate), DateTime.UtcNow.AddDays(7));

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        mockRepository.Setup(r => r.Create(It.IsAny<User>()))
            .Returns(newUser);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Create(inputCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john.doe@example.com", result.Email);
        mockRepository.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region Update Method Tests

    /// <summary>
    /// Testa que o método Update lança KeyNotFoundException quando o usuário não existe
    /// </summary>
    [Fact]
    public void Update_Should_Throw_KeyNotFoundException_When_User_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var inputUpdate = new InputUpdateUser("jane.smith@example.com", "Password123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.Update(userId, inputUpdate));
        Assert.Equal("Não foi encontrado nenhum usuário correspondente a este Id.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update lança InvalidOperationException quando a senha está incorreta
    /// </summary>
    [Fact]
    public void Update_Should_Throw_InvalidOperationException_When_Password_Is_Incorrect()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var existingUser = new User("john.doe@example.com", PasswordEncryption.Encrypt("Password123!"), DateTime.UtcNow.AddDays(7));
        var inputUpdate = new InputUpdateUser("jane.smith@example.com", "WrongPassword123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Simula que a verificação da senha falha
        // Note: Esta verificação real está na PasswordEncryption, mas para o teste podemos simular o comportamento

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.Update(userId, inputUpdate));
        Assert.Equal("Senha incorreta.", exception.Message);
    }

    /// <summary>
    /// Testa que o método Update atualiza um usuário com sucesso quando o usuário existe e a senha está correta
    /// </summary>
    [Fact]
    public void Update_Should_Update_User_When_User_Exists_And_Password_Is_Correct()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var encryptedPassword = PasswordEncryption.Encrypt("Password123!");
        var existingUser = new User("john.doe@example.com", encryptedPassword, DateTime.UtcNow.AddDays(7));
        existingUser.SetProperty(nameof(User.Id), userId);

        var updatedUser = new User("jane.smith@example.com", encryptedPassword, DateTime.UtcNow.AddDays(7));
        updatedUser.SetProperty(nameof(User.Id), userId);

        var inputUpdate = new InputUpdateUser("jane.smith@example.com", "Password123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        mockRepository.Setup(r => r.Update(It.IsAny<User>()))
            .Returns(updatedUser);

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.Update(userId, inputUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jane.smith@example.com", result.Email);
        mockRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region RedefinePassword Method Tests

    /// <summary>
    /// Testa que o método RedefinePassword lança KeyNotFoundException quando o usuário não existe
    /// </summary>
    [Fact]
    public void RedefinePassword_Should_Throw_KeyNotFoundException_When_User_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var inputRedefinePassword = new InputRedefinePasswordUser("OldPassword123!", "NewPassword123!", "NewPassword123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.RedefinePassword(userId, inputRedefinePassword));
        Assert.Equal("Não foi encontrado nenhum usuário correspondente a este Id.", exception.Message);
    }

    /// <summary>
    /// Testa que o método RedefinePassword lança InvalidOperationException quando a senha antiga está incorreta
    /// </summary>
    [Fact]
    public void RedefinePassword_Should_Throw_InvalidOperationException_When_Old_Password_Is_Incorrect()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var existingUser = new User("john.doe@example.com", "encryptedOldPassword", DateTime.UtcNow.AddDays(7));
        var inputRedefinePassword = new InputRedefinePasswordUser("WrongOldPassword123!", "NewPassword123!", "NewPassword123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => service.RedefinePassword(userId, inputRedefinePassword));
        Assert.Equal("Senha incorreta.", exception.Message);
    }

    /// <summary>
    /// Testa que o método RedefinePassword redefine a senha com sucesso quando o usuário existe e a senha antiga está correta
    /// </summary>
    [Fact]
    public void RedefinePassword_Should_Redefine_Password_When_User_Exists_And_Old_Password_Is_Correct()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var encryptedOldPassword = PasswordEncryption.Encrypt("OldPassword123!");
        var existingUser = new User("john.doe@example.com", encryptedOldPassword, DateTime.UtcNow.AddDays(7));
        existingUser.SetProperty(nameof(User.Id), userId);

        var inputRedefinePassword = new InputRedefinePasswordUser("OldPassword123!", "NewPassword123!", "NewPassword123!");

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        mockRepository.Setup(r => r.Update(It.IsAny<User>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        var result = service.RedefinePassword(userId, inputRedefinePassword);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region UpdateTokenExpirationDate Method Tests

    /// <summary>
    /// Testa que o método UpdateTokenExpirationDate lança KeyNotFoundException quando o usuário não existe
    /// </summary>
    [Fact]
    public void UpdateTokenExpirationDate_Should_Throw_KeyNotFoundException_When_User_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => service.UpdateTokenExpirationDate(userId));
        Assert.Equal("Não foi encontrado nenhum usuário correspondente a este Id.", exception.Message);
    }

    /// <summary>
    /// Testa que o método UpdateTokenExpirationDate atualiza a data de expiração do token com sucesso
    /// </summary>
    [Fact]
    public void UpdateTokenExpirationDate_Should_Update_Token_Expiration_Date()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var existingUser = new User("john.doe@example.com", "encryptedPassword", DateTime.UtcNow.AddDays(7));
        existingUser.SetProperty(nameof(User.Id), userId);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        mockRepository.Setup(r => r.Update(It.IsAny<User>()))
            .Verifiable();

        mockUnitOfWork.Setup(uow => uow.Commit())
            .Verifiable();

        // Act
        service.UpdateTokenExpirationDate(userId);

        // Assert
        mockRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    #endregion

    #region GetByEmail Method Tests

    /// <summary>
    /// Testa que o método GetByEmail retorna null quando o usuário não existe
    /// </summary>
    [Fact]
    public void GetByEmail_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var email = "nonexistent@example.com";

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act
        var result = service.GetByEmail(email);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método GetByEmail retorna um usuário quando o usuário existe
    /// </summary>
    [Fact]
    public void GetByEmail_Should_Return_User_When_User_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var email = "john.doe@example.com";
        var encryptedPassword = PasswordEncryption.Encrypt("Password123!");
        var existingUser = new User("john.doe@example.com", encryptedPassword, DateTime.UtcNow.AddDays(7));
        existingUser.SetProperty(nameof(User.Id), 1L);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Act
        var result = service.GetByEmail(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john.doe@example.com", result.Email);
        Assert.Equal(1L, result.Id);
    }

    #endregion

    #region Get Method Tests

    /// <summary>
    /// Testa que o método Get retorna null quando o usuário não existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        // Act
        var result = service.Get(userId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Testa que o método Get retorna um usuário quando o usuário existe
    /// </summary>
    [Fact]
    public void Get_Should_Return_User_When_User_Exists()
    {
        // Arrange
        var (mockUnitOfWork, mockRepository) = CreateBasicMocks<IUserRepository, User, object>();
        var service = new UserService(mockUnitOfWork.Object);

        var userId = 1L;
        var encryptedPassword = PasswordEncryption.Encrypt("Password123!");
        var existingUser = new User("john.doe@example.com", encryptedPassword, DateTime.UtcNow.AddDays(7));
        existingUser.SetProperty(nameof(User.Id), userId);

        mockRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        // Act
        var result = service.Get(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john.doe@example.com", result.Email);
        Assert.Equal(userId, result.Id);
    }

    #endregion
}