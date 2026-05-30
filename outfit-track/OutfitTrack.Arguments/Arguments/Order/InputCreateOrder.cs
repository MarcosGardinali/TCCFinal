using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputCreateOrder
{
    public long? CustomerId { get; private set; }

    [MaxLength(500, ErrorMessage = "A observação deve ter no máximo 500 caracteres.")]
    public string? Observation { get; private set; }
    public List<InputCreateOrderItem>? ListCreatedItem { get; private set; }

    public InputCreateOrder() { }

    [JsonConstructor]
    public InputCreateOrder(long customerId, string? observation, List<InputCreateOrderItem> listCreatedItem)
    {
        CustomerId = customerId;
        Observation = observation;
        ListCreatedItem = listCreatedItem;
    }
}