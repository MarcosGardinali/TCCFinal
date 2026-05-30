using OutfitTrack.Domain.Entities;
using OutfitTrack.Domain.Interfaces;

namespace OutfitTrack.Infrastructure.Repositories;

public class OrderItemRepository(OutfitTrackContext context) : BaseRepository_1<OrderItem>(context), IOrderItemRepository { }