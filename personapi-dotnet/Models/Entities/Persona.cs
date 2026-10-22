using System.ComponentModel.DataAnnotations;

namespace personapi_dotnet.Models.Entities;

// Generado con Scaffold-DbContext (Database First) y ajustado con validaciones.
public partial class Persona
{
    [Display(Name = "Cédula")]
    [Range(1, int.MaxValue, ErrorMessage = "La cédula debe ser un número positivo")]
    public int Cc { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(45)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(45)]
    public string Apellido { get; set; } = null!;

    [Display(Name = "Género")]
    [Required(ErrorMessage = "El género es obligatorio")]
    [RegularExpression("^[MF]$", ErrorMessage = "El género debe ser M o F")]
    public string Genero { get; set; } = null!;

    [Range(0, 150, ErrorMessage = "La edad debe estar entre 0 y 150")]
    public int? Edad { get; set; }

    public virtual ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();

    public virtual ICollection<Telefono> Telefonos { get; set; } = new List<Telefono>();
}
