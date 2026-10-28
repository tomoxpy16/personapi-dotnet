using personapi_dotnet.Models.Entities;

namespace personapi_dotnet.Interfaces;

/// <summary>DAO de la tabla estudios. La llave es compuesta: (idProf, ccPer).</summary>
public interface IEstudioRepository
{
    Task<List<Estudio>> GetAllAsync();
    Task<Estudio?> GetByIdAsync(int idProf, int ccPer);
    Task<bool> ExistsAsync(int idProf, int ccPer);
    Task AddAsync(Estudio estudio);
    Task UpdateAsync(Estudio estudio);
    Task DeleteAsync(int idProf, int ccPer);
}
