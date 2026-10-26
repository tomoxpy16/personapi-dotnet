using System.ComponentModel.DataAnnotations;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Models.DTOs;

/// <summary>Representación de un teléfono en la API REST.</summary>
public class TelefonoDto
{
    /// <example>3009998877</example>
    [Required, StringLength(15)]
    public string Num { get; set; } = null!;

    /// <example>Claro</example>
    [Required, StringLength(45)]
    public string Oper { get; set; } = null!;

    /// <example>1001234567</example>
    [Range(1, int.MaxValue)]
    public int Duenio { get; set; }

    public static TelefonoDto From(Telefono t) => new() { Num = t.Num, Oper = t.Oper, Duenio = t.Duenio };

    public Telefono ToEntity() => new() { Num = Num, Oper = Oper, Duenio = Duenio };
}
