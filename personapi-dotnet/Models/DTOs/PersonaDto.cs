using System.ComponentModel.DataAnnotations;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Models.DTOs;

/// <summary>Representación de una persona en la API REST (sin relaciones, para evitar ciclos en el JSON).</summary>
public class PersonaDto
{
    /// <example>1001234567</example>
    [Range(1, int.MaxValue)]
    public int Cc { get; set; }

    /// <example>Ana</example>
    [Required, StringLength(45)]
    public string Nombre { get; set; } = null!;

    /// <example>Gomez</example>
    [Required, StringLength(45)]
    public string Apellido { get; set; } = null!;

    /// <example>F</example>
    [Required, RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F")]
    public string Genero { get; set; } = null!;

    /// <example>22</example>
    [Range(0, 150)]
    public int? Edad { get; set; }

    public static PersonaDto From(Persona p) => new()
    {
        Cc = p.Cc, Nombre = p.Nombre, Apellido = p.Apellido, Genero = p.Genero, Edad = p.Edad
    };

    public Persona ToEntity() => new()
    {
        Cc = Cc, Nombre = Nombre, Apellido = Apellido, Genero = Genero, Edad = Edad
    };
}
