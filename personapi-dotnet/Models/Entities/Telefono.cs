using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace personapi_dotnet.Models.Entities;

public partial class Telefono
{
    [Display(Name = "Número")]
    [Required(ErrorMessage = "El número es obligatorio")]
    [StringLength(15)]
    public string Num { get; set; } = null!;

    [Display(Name = "Operador")]
    [Required(ErrorMessage = "El operador es obligatorio")]
    [StringLength(45)]
    public string Oper { get; set; } = null!;

    [Display(Name = "Dueño")]
    public int Duenio { get; set; }

    [ValidateNever]
    public virtual Persona DuenioNavigation { get; set; } = null!;
}
