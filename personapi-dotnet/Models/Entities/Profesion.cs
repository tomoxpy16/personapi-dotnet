using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.Entities;

public partial class Profesion
{
    [Display(Name = "Id")]
    [Range(1, 999999, ErrorMessage = "El id debe estar entre 1 y 999999")]
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(90)]
    public string Nom { get; set; } = null!;

    [Display(Name = "Descripción")]
    public string? Des { get; set; }

    public virtual ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();
}
