using Microsoft.AspNetCore.Mvc;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>Controlador MVC (vistas Razor) del CRUD de profesion.</summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class ProfesionesController : Controller
{
    private readonly IProfesionRepository _repo;

    public ProfesionesController(IProfesionRepository repo)
    {
        _repo = repo;
    }

    // GET: /Profesiones
    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    // GET: /Profesiones/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var profesion = await _repo.GetByIdAsync(id);
        return profesion is null ? NotFound() : View(profesion);
    }

    // GET: /Profesiones/Create
    public IActionResult Create() => View();

    // POST: /Profesiones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nom,Des")] Profesion profesion)
    {
        if (await _repo.ExistsAsync(profesion.Id))
            ModelState.AddModelError(nameof(Profesion.Id), "Ya existe una profesión con ese id");

        if (!ModelState.IsValid) return View(profesion);

        await _repo.AddAsync(profesion);
        TempData["Mensaje"] = "Profesión creada";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Profesiones/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var profesion = await _repo.GetByIdAsync(id);
        return profesion is null ? NotFound() : View(profesion);
    }

    // POST: /Profesiones/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Des")] Profesion profesion)
    {
        if (id != profesion.Id) return BadRequest();
        if (!await _repo.ExistsAsync(id)) return NotFound();
        if (!ModelState.IsValid) return View(profesion);

        await _repo.UpdateAsync(profesion);
        TempData["Mensaje"] = "Profesión actualizada";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Profesiones/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var profesion = await _repo.GetByIdAsync(id);
        return profesion is null ? NotFound() : View(profesion);
    }

    // POST: /Profesiones/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repo.DeleteAsync(id);
        TempData["Mensaje"] = "Profesión eliminada";
        return RedirectToAction(nameof(Index));
    }
}
