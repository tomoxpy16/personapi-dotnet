using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Api;

/// <summary>Endpoints REST del CRUD de profesion.</summary>
[ApiController]
[Route("api/profesiones")]
[Produces("application/json")]
public class ProfesionesApiController : ControllerBase
{
    private readonly IProfesionRepository _repo;

    public ProfesionesApiController(IProfesionRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Lista todas las profesiones.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfesionDto>>> GetAll() =>
        Ok((await _repo.GetAllAsync()).Select(ProfesionDto.From).ToList());

    /// <summary>Obtiene una profesión por su id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfesionDto>> GetById(int id)
    {
        var profesion = await _repo.GetByIdAsync(id);
        return profesion is null ? NotFound() : ProfesionDto.From(profesion);
    }

    /// <summary>Crea una profesión.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfesionDto>> Create(ProfesionDto dto)
    {
        if (await _repo.ExistsAsync(dto.Id))
            return Conflict($"Ya existe una profesión con id {dto.Id}");

        await _repo.AddAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Actualiza una profesión existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, ProfesionDto dto)
    {
        if (id != dto.Id) return BadRequest("El id de la URL no coincide con el del cuerpo");
        if (!await _repo.ExistsAsync(id)) return NotFound();

        await _repo.UpdateAsync(dto.ToEntity());
        return NoContent();
    }

    /// <summary>Elimina una profesión junto con los estudios que la referencian.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repo.ExistsAsync(id)) return NotFound();

        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
