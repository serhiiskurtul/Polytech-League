using Microsoft.EntityFrameworkCore;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations;

public class CollegeService : ICollegeService
{
    private readonly ApplicationDbContext _context;

    public CollegeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<College>> GetAllCollegesAsync()
    {
        return await _context.Colleges.ToListAsync();
    }

    public async Task<College?> GetCollegeByIdAsync(long id)
    {
        return await _context.Colleges.FindAsync(id);
    }

    public async Task<College> CreateCollegeAsync(College college)
    {
        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();
        return college;
    }

    public async Task<College> UpdateCollegeAsync(long id, College college)
    {
        var existing = await _context.Colleges.FindAsync(id);
        if (existing == null) throw new Exception("College not found");

        existing.Name = college.Name;
        existing.ShortName = college.ShortName;
        existing.Region = college.Region;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteCollegeAsync(long id)
    {
        var existing = await _context.Colleges.FindAsync(id);
        if (existing != null)
        {
            _context.Colleges.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
