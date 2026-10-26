using Microsoft.EntityFrameworkCore;
using personapi_dotnet.Interfaces;
using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Repository;

/// <summary>Implementación del DAO de persona con Entity Framework Core.</summary>
public class PersonaRepository : IPersonaRepository
{
    private readonly PersonaDbContext _context;

    public PersonaRepository(PersonaDbContext context)
    {
        _context = context;
    }

    public Task<List<Persona>> GetAllAsync() =>
        _context.Personas.AsNoTracking().OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToListAsync();

    public Task<Persona?> GetByIdAsync(int cc) =>
        _context.Personas.AsNoTracking()
            .Include(p => p.Telefonos)
            .Include(p => p.Estudios).ThenInclude(e => e.IdProfNavigation)
            .FirstOrDefaultAsync(p => p.Cc == cc);

    public Task<bool> ExistsAsync(int cc) => _context.Personas.AnyAsync(p => p.Cc == cc);

    public async Task AddAsync(Persona persona)
    {
        _context.Personas.Add(persona);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Persona persona)
    {
        _context.Personas.Update(persona);
        await _context.SaveChangesAsync();
    }

    // Las FK no tienen ON DELETE CASCADE, así que se borran primero los teléfonos y estudios
    // de la persona. SaveChanges ejecuta todo en una sola transacción.
    public async Task DeleteAsync(int cc)
    {
        var persona = await _context.Personas
            .Include(p => p.Telefonos)
            .Include(p => p.Estudios)
            .FirstOrDefaultAsync(p => p.Cc == cc);
        if (persona is null) return;

        _context.Telefonos.RemoveRange(persona.Telefonos);
        _context.Estudios.RemoveRange(persona.Estudios);
        _context.Personas.Remove(persona);
        await _context.SaveChangesAsync();
    }
}
