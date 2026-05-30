using OutfitTrack.Arguments.Enums;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputFilterOrder
{
    public long? Number { get; set; }

    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string? CustomerName { get; set; }

    public EnumStatusOrder? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public long? ProductId { get; set; }

    [MaxLength(50, ErrorMessage = "A variação deve ter no máximo 50 caracteres.")]
    public string? Variation { get; set; }
    public EnumStatusOrderItem? ItemStatus { get; set; }

    public EnumOrderByOrder? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}