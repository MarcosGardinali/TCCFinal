using OutfitTrack.Arguments.Enums;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputFilterProduct
{
    [MaxLength(20, ErrorMessage = "O código deve ter no máximo 20 caracteres.")]
    public string? Code { get; set; }

    [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
    public string? Description { get; set; }

    [MaxLength(100, ErrorMessage = "A categoria deve ter no máximo 100 caracteres.")]
    public string? Category { get; set; }

    [MaxLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres.")]
    public string? Brand { get; set; }

    public EnumOrderByProduct? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}