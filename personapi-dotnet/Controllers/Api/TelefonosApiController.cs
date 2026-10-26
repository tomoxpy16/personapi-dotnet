using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.DTOs;

namespace personapi_dotnet.Controllers.Api;

/// <summary>Endpoints REST del CRUD de telefono.</summary>
[ApiController]
[Route("api/telefonos")]
[Produces("application/json")]
public class TelefonosApiController : ControllerBase
{
    private readonly ITelefonoRepository _repo;
    private readonly IPersonaRepository _personas;

    public TelefonosApiController(ITelefonoRepository repo, IPersonaRepository personas)
    {
        _repo = repo;
        _personas = personas;
    }

    /// <summary>Lista todos los teléfonos.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TelefonoDto>>> GetAll() =>
        Ok((await _repo.GetAllAsync()).Select(TelefonoDto.From).ToList());

    /// <summary>Obtiene un teléfono por su número.</summary>
    [HttpGet("{num}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TelefonoDto>> GetById(string num)
    {
        var telefono = await _repo.GetByIdAsync(num);
        return telefono is null ? NotFound() : TelefonoDto.From(telefono);
    }

    /// <summary>Crea un teléfono. El dueño debe existir.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TelefonoDto>> Create(TelefonoDto dto)
    {
        if (!await _personas.ExistsAsync(dto.Duenio)) return BadRequest($"No existe la persona {dto.Duenio}");
        if (await _repo.ExistsAsync(dto.Num)) return Conflict($"Ya existe el teléfono {dto.Num}");

        await _repo.AddAsync(dto.ToEntity());
        return CreatedAtAction(nameof(GetById), new { num = dto.Num }, dto);
    }

    /// <summary>Actualiza el operador o el dueño de un teléfono.</summary>
    [HttpPut("{num}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string num, TelefonoDto dto)
    {
        if (num != dto.Num) return BadRequest("El número de la URL no coincide con el del cuerpo");
        if (!await _repo.ExistsAsync(num)) return NotFound();
        if (!await _personas.ExistsAsync(dto.Duenio)) return BadRequest($"No existe la persona {dto.Duenio}");

        await _repo.UpdateAsync(dto.ToEntity());
        return NoContent();
    }

    /// <summary>Elimina un teléfono.</summary>
    [HttpDelete("{num}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string num)
    {
        if (!await _repo.ExistsAsync(num)) return NotFound();

        await _repo.DeleteAsync(num);
        return NoContent();
    }
}
