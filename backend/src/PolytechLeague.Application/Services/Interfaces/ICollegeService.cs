using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Application.Services.Interfaces;

public interface ICollegeService
{
    Task<IEnumerable<College>> GetAllCollegesAsync();
    Task<College?> GetCollegeByIdAsync(long id);
    Task<College> CreateCollegeAsync(College college);
    Task<College> UpdateCollegeAsync(long id, College college);
    Task DeleteCollegeAsync(long id);
}
