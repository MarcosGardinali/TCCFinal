using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputUpdateOrderItem
{
    [MaxLength(50, ErrorMessage = "A variação deve ter no máximo 50 caracteres.")]
    public string? Variation { get; private set; }
    public EnumStatusOrderItem? Status { get; private set; }

    public InputUpdateOrderItem() { }

    [JsonConstructor]
    public InputUpdateOrderItem(string variation, EnumStatusOrderItem status)
    {
        Variation = variation;
        Status = status;
    }
}