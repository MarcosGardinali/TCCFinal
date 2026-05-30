using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class UserRepository(OutfitTrackContext context) : BaseRepository_1<User>(context), IUserRepository { }