using Microsoft.AspNetCore.Mvc;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollegesController : ControllerBase
{
    private readonly ICollegeService _collegeService;
    private readonly IFileService _fileService;

    public CollegesController(ICollegeService collegeService, IFileService fileService)
    {
        _collegeService = collegeService;
        _fileService = fileService;
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Admin")]
    [HttpPost("{id}/upload-logo")]
    public async Task<IActionResult> UploadLogo(long id, IFormFile file, CancellationToken cancellationToken)
    {
        var college = await _collegeService.GetCollegeByIdAsync(id);
        if (college == null) return NotFound();

        // Delete old logo if it exists and was uploaded to our storage
        if (!string.IsNullOrEmpty(college.LogoUrl) && college.LogoUrl.Contains("/colleges/logos/"))
        {
            try { _fileService.DeleteFile(college.LogoUrl); } catch { }
        }

        var url = await _fileService.SaveFileAsync(file, "colleges/logos", cancellationToken);
        college.LogoUrl = url;
        
        await _collegeService.UpdateCollegeAsync(id, college);
        return Ok(new { url });
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

    [HttpPost]
    public async Task<ActionResult<College>> Create(College college)
    {
        var created = await _collegeService.CreateCollegeAsync(college);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _collegeService.DeleteCollegeAsync(id);
        return NoContent();
    }
}
