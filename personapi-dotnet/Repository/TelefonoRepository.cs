using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>Implementación del DAO de telefono con Entity Framework Core.</summary>
public class TelefonoRepository : ITelefonoRepository
{
    private readonly PersonaDbContext _context;

    public TelefonoRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Telefono>> GetAllAsync() =>
        _context.Telefonos.AsNoTracking()
            .Include(t => t.DuenioNavigation)
            .OrderBy(t => t.Num)
            .ToListAsync();

    public Task<Telefono?> GetByIdAsync(string num) =>
        _context.Telefonos.AsNoTracking()
            .Include(t => t.DuenioNavigation)
            .FirstOrDefaultAsync(t => t.Num == num);

    public Task<bool> ExistsAsync(string num) => _context.Telefonos.AnyAsync(t => t.Num == num);

    public async Task AddAsync(Telefono telefono)
    {
        _context.Telefonos.Add(telefono);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Telefono telefono)
    {
        _context.Telefonos.Update(telefono);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string num)
    {
        var telefono = await _context.Telefonos.FindAsync(num);
        if (telefono is null) return;

        _context.Telefonos.Remove(telefono);
        await _context.SaveChangesAsync();
    }
}
