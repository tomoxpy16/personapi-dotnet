using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Api;

/// <summary>Endpoints REST del CRUD de estudios. La llave compuesta va en la URL: /{idProf}/{ccPer}.</summary>
[ApiController]
[Route("api/estudios")]
[Produces("application/json")]
public class EstudiosApiController : ControllerBase
{
    private readonly IEstudioRepository _repo;
    private readonly IPersonaRepository _personas;
    private readonly IProfesionRepository _profesiones;

    public EstudiosApiController(IEstudioRepository repo, IPersonaRepository personas, IProfesionRepository profesiones)
    {
        _repo = repo;
        _personas = personas;
        _profesiones = profesiones;
    }

    /// <summary>Lista todos los estudios.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstudioDto>>> GetAll() =>
        Ok((await _repo.GetAllAsync()).Select(EstudioDto.From).ToList());

    /// <summary>Obtiene un estudio por profesión y cédula.</summary>
    [HttpGet("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudioDto>> GetById(int idProf, int ccPer)
    {
        var estudio = await _repo.GetByIdAsync(idProf, ccPer);
        return estudio is null ? NotFound() : EstudioDto.From(estudio);
    }

    /// <summary>Registra un estudio. La persona y la profesión deben existir.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudioDto>> Create(EstudioDto dto)
    {
        if (!await _personas.ExistsAsync(dto.CcPer)) return BadRequest($"No existe la persona {dto.CcPer}");
        if (!await _profesiones.ExistsAsync(dto.IdProf)) return BadRequest($"No existe la profesión {dto.IdProf}");
        if (await _repo.ExistsAsync(dto.IdProf, dto.CcPer))
            return Conflict("Esa persona ya tiene registrado ese estudio");

        await _repo.AddAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { idProf = dto.IdProf, ccPer = dto.CcPer }, dto);
    }

    /// <summary>Actualiza la fecha y la universidad de un estudio.</summary>
    [HttpPut("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int idProf, int ccPer, EstudioDto dto)
    {
        if (idProf != dto.IdProf || ccPer != dto.CcPer)
            return BadRequest("La llave de la URL no coincide con la del cuerpo");
        if (!await _repo.ExistsAsync(idProf, ccPer)) return NotFound();

        await _repo.UpdateAsync(dto.ToEntity());
        return NoContent();
    }

    /// <summary>Elimina un estudio.</summary>
    [HttpDelete("{idProf:int}/{ccPer:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int idProf, int ccPer)
    {
        if (!await _repo.ExistsAsync(idProf, ccPer)) return NotFound();

        await _repo.DeleteAsync(idProf, ccPer);
        return NoContent();
    }
}
