using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace personapi_dotnet.Models.Entities;

// Tabla estudios: llave primaria compuesta (IdProf, CcPer).
public partial class Estudio
{
    [Display(Name = "Profesión")]
    public int IdProf { get; set; }

    [Display(Name = "Persona")]
    public int CcPer { get; set; }

    [Display(Name = "Fecha de grado")]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    [Display(Name = "Universidad")]
    [StringLength(50)]
    public string? Univer { get; set; }

    [ValidateNever]
    public virtual Persona CcPerNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Profesion IdProfNavigation { get; set; } = null!;
}
