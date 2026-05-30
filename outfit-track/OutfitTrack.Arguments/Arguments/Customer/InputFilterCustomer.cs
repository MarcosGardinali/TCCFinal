using OutfitTrack.Arguments.Enums;
using System.ComponentModel.DataAnnotations;

namespace OutfitTrack.Arguments;

public class InputFilterCustomer
{
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string? Name { get; set; }

    [RegularExpression(@"^\d{1,11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos numéricos.")]
    public string? Cpf { get; set; }

    [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
    [MaxLength(256, ErrorMessage = "O e-mail deve ter no máximo 256 caracteres.")]
    public string? Email { get; set; }

    [RegularExpression(@"^\d{1,13}$", ErrorMessage = "O número de celular deve ter no máximo 13 caracteres numéricos.")]
    public string? MobilePhoneNumber { get; set; }

    public EnumOrderByCustomer? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }
}