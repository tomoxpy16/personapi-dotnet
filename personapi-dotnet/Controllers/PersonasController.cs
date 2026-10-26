using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>Controlador MVC (vistas Razor) del CRUD de persona.</summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class PersonasController : Controller
{
    private readonly IPersonaRepository _repo;

    public PersonasController(IPersonaRepository repo)
    {
        _repo = repo;
    }

    // GET: /Personas
    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    // GET: /Personas/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var persona = await _repo.GetByIdAsync(id);
        return persona is null ? NotFound() : View(persona);
    }

    // GET: /Personas/Create
    public IActionResult Create() => View();

    // POST: /Personas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Cc,Nombre,Apellido,Genero,Edad")] Persona persona)
    {
        if (await _repo.ExistsAsync(persona.Cc))
            ModelState.AddModelError(nameof(Persona.Cc), "Ya existe una persona con esa cédula");

        if (!ModelState.IsValid) return View(persona);

        await _repo.AddAsync(persona);
        TempData["Mensaje"] = "Persona creada";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Personas/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var persona = await _repo.GetByIdAsync(id);
        return persona is null ? NotFound() : View(persona);
    }

    // POST: /Personas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Cc,Nombre,Apellido,Genero,Edad")] Persona persona)
    {
        if (id != persona.Cc) return BadRequest();
        if (!await _repo.ExistsAsync(id)) return NotFound();
        if (!ModelState.IsValid) return View(persona);

        await _repo.UpdateAsync(persona);
        TempData["Mensaje"] = "Persona actualizada";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Personas/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var persona = await _repo.GetByIdAsync(id);
        return persona is null ? NotFound() : View(persona);
    }

    // POST: /Personas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repo.DeleteAsync(id);
        TempData["Mensaje"] = "Persona eliminada";
        return RedirectToAction(nameof(Index));
    }
}
