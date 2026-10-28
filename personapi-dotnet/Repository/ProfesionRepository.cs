using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>Implementación del DAO de profesion con Entity Framework Core.</summary>
public class ProfesionRepository : IProfesionRepository
{
    private readonly PersonaDbContext _context;

    public ProfesionRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Profesion>> GetAllAsync() =>
        _context.Profesions.AsNoTracking().OrderBy(p => p.Id).ToListAsync();

    public Task<Profesion?> GetByIdAsync(int id) =>
        _context.Profesions.AsNoTracking()
            .Include(p => p.Estudios).ThenInclude(e => e.CcPerNavigation)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<bool> ExistsAsync(int id) => _context.Profesions.AnyAsync(p => p.Id == id);

    public async Task AddAsync(Profesion profesion)
    {
        _context.Profesions.Add(profesion);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profesion profesion)
    {
        _context.Profesions.Update(profesion);
        await _context.SaveChangesAsync();
    }

    // Igual que en persona: se borran primero los estudios que referencian la profesión.
    public async Task DeleteAsync(int id)
    {
        var profesion = await _context.Profesions
            .Include(p => p.Estudios)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (profesion is null) return;

        _context.Estudios.RemoveRange(profesion.Estudios);
        _context.Profesions.Remove(profesion);
        await _context.SaveChangesAsync();
    }
}
