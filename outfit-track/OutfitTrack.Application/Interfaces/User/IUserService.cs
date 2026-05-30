using OutfitTrack.Arguments;

namespace OutfitTrack.Application.Interfaces;

public interface IUserService : IBaseService_1<InputCreateUser, InputUpdateUser, OutputUser>
{
    void UpdateTokenExpirationDate(long id);
    bool RedefinePassword(long id, InputRedefinePasswordUser inputRedefinePassword);
    OutputUser? GetByEmail(string email);
}