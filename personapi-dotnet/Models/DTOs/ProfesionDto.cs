using System.ComponentModel.DataAnnotations;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Models.DTOs;

/// <summary>Representación de una profesión en la API REST.</summary>
public class ProfesionDto
{
    /// <example>5</example>
    [Range(1, 999999)]
    public int Id { get; set; }

    /// <example>Arquitectura</example>
    [Required, StringLength(90)]
    public string Nom { get; set; } = null!;

    /// <example>Diseño de edificaciones y espacios urbanos.</example>
    public string? Des { get; set; }

    public static ProfesionDto From(Profesion p) => new() { Id = p.Id, Nom = p.Nom, Des = p.Des };

    public Profesion ToEntity() => new() { Id = Id, Nom = Nom, Des = Des };
}
