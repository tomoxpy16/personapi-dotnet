using System.ComponentModel.DataAnnotations;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Models.DTOs;

/// <summary>Representación de un estudio (persona + profesión) en la API REST.</summary>
public class EstudioDto
{
    /// <example>2</example>
    [Range(1, int.MaxValue)]
    public int IdProf { get; set; }

    /// <example>1001234567</example>
    [Range(1, int.MaxValue)]
    public int CcPer { get; set; }

    /// <example>2025-06-30</example>
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    /// <example>Universidad Nacional de Colombia</example>
    [StringLength(50)]
    public string? Univer { get; set; }

    public static EstudioDto From(Estudio e) => new()
    {
        IdProf = e.IdProf, CcPer = e.CcPer, Fecha = e.Fecha, Univer = e.Univer
    };

    public Estudio ToEntity() => new()
    {
        IdProf = IdProf, CcPer = CcPer, Fecha = Fecha, Univer = Univer
    };
}
