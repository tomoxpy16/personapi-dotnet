using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Controllers;

/// <summary>Controlador MVC del CRUD de estudios. La llave compuesta llega como ?idProf=..&amp;ccPer=..</summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class EstudiosController : Controller
{
    private readonly IEstudioRepository _repo;
    private readonly IPersonaRepository _personas;
    private readonly IProfesionRepository _profesiones;

    public EstudiosController(IEstudioRepository repo, IPersonaRepository personas, IProfesionRepository profesiones)
    {
        _repo = repo;
        _personas = personas;
        _profesiones = profesiones;
    }

    // GET: /Estudios
    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    // GET: /Estudios/Details?idProf=1&ccPer=1001234567
    public async Task<IActionResult> Details(int idProf, int ccPer)
    {
        var estudio = await _repo.GetByIdAsync(idProf, ccPer);
        return estudio is null ? NotFound() : View(estudio);
    }

    // GET: /Estudios/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    // POST: /Estudios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdProf,CcPer,Fecha,Univer")] Estudio estudio)
    {
        if (await _repo.ExistsAsync(estudio.IdProf, estudio.CcPer))
            ModelState.AddModelError(string.Empty, "Esa persona ya tiene registrado ese estudio");

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(estudio);
            return View(estudio);
        }

        await _repo.AddAsync(estudio);
        TempData["Mensaje"] = "Estudio registrado";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Estudios/Edit?idProf=1&ccPer=1001234567
    public async Task<IActionResult> Edit(int idProf, int ccPer)
    {
        var estudio = await _repo.GetByIdAsync(idProf, ccPer);
        return estudio is null ? NotFound() : View(estudio);
    }

    // POST: /Estudios/Edit  (la llave no se puede cambiar; solo fecha y universidad)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("IdProf,CcPer,Fecha,Univer")] Estudio estudio)
    {
        if (!await _repo.ExistsAsync(estudio.IdProf, estudio.CcPer)) return NotFound();
        if (!ModelState.IsValid) return View(estudio);

        await _repo.UpdateAsync(estudio);
        TempData["Mensaje"] = "Estudio actualizado";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Estudios/Delete?idProf=1&ccPer=1001234567
    public async Task<IActionResult> Delete(int idProf, int ccPer)
    {
        var estudio = await _repo.GetByIdAsync(idProf, ccPer);
        return estudio is null ? NotFound() : View(estudio);
    }

    // POST: /Estudios/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int idProf, int ccPer)
    {
        await _repo.DeleteAsync(idProf, ccPer);
        TempData["Mensaje"] = "Estudio eliminado";
        return RedirectToAction(nameof(Index));
    }

    // Listas desplegables de personas y profesiones para el formulario de creación.
    private async Task CargarListasAsync(Estudio? estudio = null)
    {
        var personas = (await _personas.GetAllAsync())
            .Select(p => new { p.Cc, Texto = $"{p.Cc} - {p.Nombre} {p.Apellido}" });
        var profesiones = await _profesiones.GetAllAsync();

        ViewData["CcPer"] = new SelectList(personas, "Cc", "Texto", estudio?.CcPer);
        ViewData["IdProf"] = new SelectList(profesiones, nameof(Profesion.Id), nameof(Profesion.Nom), estudio?.IdProf);
    }
}
