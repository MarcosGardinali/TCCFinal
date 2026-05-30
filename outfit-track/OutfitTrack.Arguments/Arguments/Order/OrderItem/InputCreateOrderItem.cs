using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputCreateOrderItem
{
    public long? ProductId { get; private set; }

    [MaxLength(50, ErrorMessage = "A variação deve ter no máximo 50 caracteres.")]
    public string? Variation { get; private set; }

    public InputCreateOrderItem() { }

    [JsonConstructor]
    public InputCreateOrderItem(long productId, string variation)
    {
        ProductId = productId;
        Variation = variation;
    }
}