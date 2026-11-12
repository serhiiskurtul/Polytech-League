using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;
    
    public CollegesController(ICollegeService collegeService)
    {
        _collegeService = collegeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<College>>> GetAll()
    {
        return Ok(await _collegeService.GetAllCollegesAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<College>> GetById(long id)
    {
        var college = await _collegeService.GetCollegeByIdAsync(id);
        if (college == null) return NotFound();
        return Ok(college);
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<ActionResult<College>> Create(College college)
    {
        var created = await _collegeService.CreateCollegeAsync(college);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<College>> Update(long id, College college)
    {
        try
        {
            return Ok(await _collegeService.UpdateCollegeAsync(id, college));
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _collegeService.DeleteCollegeAsync(id);
        return NoContent();
    }
}
