using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>DAO de la tabla profesion.</summary>
public interface IProfesionRepository
{
    Task<List<Profesion>> GetAllAsync();
    Task<Profesion?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Profesion profesion);
    Task UpdateAsync(Profesion profesion);
    Task DeleteAsync(int id);
}
