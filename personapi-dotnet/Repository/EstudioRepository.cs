using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>Implementación del DAO de estudios con Entity Framework Core.</summary>
public class EstudioRepository : IEstudioRepository
{
    private readonly PersonaDbContext _context;

    public EstudioRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Estudio>> GetAllAsync() =>
        _context.Estudios.AsNoTracking()
            .Include(e => e.CcPerNavigation)
            .Include(e => e.IdProfNavigation)
            .OrderBy(e => e.CcPer).ThenBy(e => e.IdProf)
            .ToListAsync();

    public Task<Estudio?> GetByIdAsync(int idProf, int ccPer) =>
        _context.Estudios.AsNoTracking()
            .Include(e => e.CcPerNavigation)
            .Include(e => e.IdProfNavigation)
            .FirstOrDefaultAsync(e => e.IdProf == idProf && e.CcPer == ccPer);

    public Task<bool> ExistsAsync(int idProf, int ccPer) =>
        _context.Estudios.AnyAsync(e => e.IdProf == idProf && e.CcPer == ccPer);

    public async Task AddAsync(Estudio estudio)
    {
        _context.Estudios.Add(estudio);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Estudio estudio)
    {
        _context.Estudios.Update(estudio);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int idProf, int ccPer)
    {
        var estudio = await _context.Estudios.FindAsync(idProf, ccPer);
        if (estudio is null) return;

        _context.Estudios.Remove(estudio);
        await _context.SaveChangesAsync();
    }
}
