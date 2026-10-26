using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>Controlador MVC (vistas Razor) del CRUD de telefono.</summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class TelefonosController : Controller
{
    private readonly ITelefonoRepository _repo;
    private readonly IPersonaRepository _personas;

    public TelefonosController(ITelefonoRepository repo, IPersonaRepository personas)
    {
        _repo = repo;
        _personas = personas;
    }

    // GET: /Telefonos
    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    // GET: /Telefonos/Details/3001112233
    public async Task<IActionResult> Details(string id)
    {
        var telefono = await _repo.GetByIdAsync(id);
        return telefono is null ? NotFound() : View(telefono);
    }

    // GET: /Telefonos/Create
    public async Task<IActionResult> Create()
    {
        await CargarDueniosAsync();
        return View();
    }

    // POST: /Telefonos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Num,Oper,Duenio")] Telefono telefono)
    {
        if (await _repo.ExistsAsync(telefono.Num))
            ModelState.AddModelError(nameof(Telefono.Num), "Ese número ya está registrado");

        if (!ModelState.IsValid)
        {
            await CargarDueniosAsync(telefono.Duenio);
            return View(telefono);
        }

        await _repo.AddAsync(telefono);
        TempData["Mensaje"] = "Teléfono creado";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Telefonos/Edit/3001112233
    public async Task<IActionResult> Edit(string id)
    {
        var telefono = await _repo.GetByIdAsync(id);
        if (telefono is null) return NotFound();

        await CargarDueniosAsync(telefono.Duenio);
        return View(telefono);
    }

    // POST: /Telefonos/Edit/3001112233
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Num,Oper,Duenio")] Telefono telefono)
    {
        if (id != telefono.Num) return BadRequest();
        if (!await _repo.ExistsAsync(id)) return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarDueniosAsync(telefono.Duenio);
            return View(telefono);
        }

        await _repo.UpdateAsync(telefono);
        TempData["Mensaje"] = "Teléfono actualizado";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Telefonos/Delete/3001112233
    public async Task<IActionResult> Delete(string id)
    {
        var telefono = await _repo.GetByIdAsync(id);
        return telefono is null ? NotFound() : View(telefono);
    }

    // POST: /Telefonos/Delete/3001112233
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _repo.DeleteAsync(id);
        TempData["Mensaje"] = "Teléfono eliminado";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarDueniosAsync(int? seleccionado = null)
    {
        var personas = (await _personas.GetAllAsync())
            .Select(p => new { p.Cc, Texto = $"{p.Cc} - {p.Nombre} {p.Apellido}" });
        ViewData["Duenio"] = new SelectList(personas, "Cc", "Texto", seleccionado);
    }
}
