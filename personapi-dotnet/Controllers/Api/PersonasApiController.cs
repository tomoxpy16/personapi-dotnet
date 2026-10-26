using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Api;

/// <summary>Endpoints REST del CRUD de persona.</summary>
[ApiController]
[Route("api/personas")]
[Produces("application/json")]
public class PersonasApiController : ControllerBase
{
    private readonly IPersonaRepository _repo;

    public PersonasApiController(IPersonaRepository repo)
    {
        _repo = repo;
    }

    /// <summary>Lista todas las personas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonaDto>>> GetAll() =>
        Ok((await _repo.GetAllAsync()).Select(PersonaDto.From).ToList());

    /// <summary>Obtiene una persona por su cédula.</summary>
    [HttpGet("{cc:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonaDto>> GetById(int cc)
    {
        var persona = await _repo.GetByIdAsync(cc);
        return persona is null ? NotFound() : PersonaDto.From(persona);
    }

    /// <summary>Crea una persona.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonaDto>> Create(PersonaDto dto)
    {
        if (await _repo.ExistsAsync(dto.Cc))
            return Conflict($"Ya existe una persona con cédula {dto.Cc}");

        await _repo.AddAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { cc = dto.Cc }, dto);
    }

    /// <summary>Actualiza una persona existente.</summary>
    [HttpPut("{cc:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int cc, PersonaDto dto)
    {
        if (cc != dto.Cc) return BadRequest("La cédula de la URL no coincide con la del cuerpo");
        if (!await _repo.ExistsAsync(cc)) return NotFound();

        await _repo.UpdateAsync(dto.ToEntity());
        return NoContent();
    }

    /// <summary>Elimina una persona junto con sus teléfonos y estudios.</summary>
    [HttpDelete("{cc:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int cc)
    {
        if (!await _repo.ExistsAsync(cc)) return NotFound();

        await _repo.DeleteAsync(cc);
        return NoContent();
    }
}
