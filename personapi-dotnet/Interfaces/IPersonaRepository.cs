using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>DAO de la tabla persona: define QUÉ operaciones de datos existen, no CÓMO se hacen.</summary>
public interface IPersonaRepository
{
    Task<List<Persona>> GetAllAsync();
    Task<Persona?> GetByIdAsync(int cc);
    Task<bool> ExistsAsync(int cc);
    Task AddAsync(Persona persona);
    Task UpdateAsync(Persona persona);
    Task DeleteAsync(int cc);
}
