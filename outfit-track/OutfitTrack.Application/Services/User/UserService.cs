using OutfitTrack.Application.Interfaces;
using OutfitTrack.Application.Security;
using OutfitTrack.Arguments;
using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Application.Services;

public class UserService(IUnitOfWork unitOfWork) : BaseService_1<IUserRepository, InputCreateUser, InputUpdateUser, User, OutputUser>(unitOfWork), IUserService
{
    public OutputUser? GetByEmail(string email)
    {
        var user = _repository!.Get(x => x.Email == email);
        return user != null ? FromEntityToOutput(user) : null;
    }

    public override OutputUser Create(InputCreateUser inputCreate)
    {
        ValidateEmailUniqueness(inputCreate.Email!);

        return ExecuteWithCommit(() =>
        {
            var user = FromInputCreateToEntity(inputCreate);
            user.SetProperty(nameof(User.TokenExpirationDate), DateTime.UtcNow.AddDays(7));
            user.SetProperty(nameof(User.Password), PasswordEncryption.Encrypt(user.Password!));

            var createdUser = _repository!.Create(user) ??
                throw new InvalidOperationException("Falha ao criar o usuário.");

            return FromEntityToOutput(createdUser);
        });
    }

    public override OutputUser Update(long id, InputUpdateUser inputUpdate)
    {
        var user = GetEntityById(id);
        ValidatePassword(inputUpdate.Password!, user.Password!);

        return ExecuteWithCommit(() =>
        {
            user.SetProperty(nameof(User.Email), inputUpdate.Email);
            var updatedUser = _repository!.Update(user) ??
                throw new InvalidOperationException("Falha ao atualizar o usuário.");

            return FromEntityToOutput(updatedUser);
        });
    }

    public bool RedefinePassword(long id, InputRedefinePasswordUser inputRedefinePassword)
    {
        var user = GetEntityById(id);
        ValidatePassword(inputRedefinePassword.Password!, user.Password!);

        ExecuteWithCommit(() =>
        {
            user.SetProperty(nameof(User.Password), PasswordEncryption.Encrypt(inputRedefinePassword.NewPassword!));
            _repository!.Update(user);
        });

        return true;
    }

    public void UpdateTokenExpirationDate(long id)
    {
        var user = GetEntityById(id);

        ExecuteWithCommit(() =>
        {
            user.SetProperty(nameof(User.TokenExpirationDate), DateTime.UtcNow.AddDays(7));
            _repository!.Update(user);
        });
    }

    #region Business Validations
    private void ValidateEmailUniqueness(string email)
    {
        var existingUser = _repository!.Get(x => x.Email == email);
        if (existingUser != null)
            throw new InvalidOperationException($"E-mail '{email}' já cadastrado.");
    }

    private static void ValidatePassword(string inputPassword, string storedPassword)
    {
        if (!PasswordEncryption.Verify(inputPassword, storedPassword))
            throw new InvalidOperationException("Senha incorreta.");
    }

    private User GetEntityById(long id)
    {
        return _repository!.Get(x => x.Id == id) ??
            throw new KeyNotFoundException("Não foi encontrado nenhum usuário correspondente a este Id.");
    }
    #endregion

    #region Required Mappers
    public override OutputUser FromEntityToOutput(User entity)
    {
        return new OutputUser
        {
            Id = entity.Id,
            Email = entity.Email,
            TokenExpirationDate = entity.TokenExpirationDate
        };
    }

    public override User FromInputCreateToEntity(InputCreateUser inputCreate)
    {
        var entity = new User();
        entity.SetProperty(nameof(User.Email), inputCreate.Email);
        entity.SetProperty(nameof(User.Password), inputCreate.Password);
        return entity;
    }
    #endregion
}