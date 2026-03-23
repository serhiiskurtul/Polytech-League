using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Domain.Params;
using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers.TableTennis;

[ApiController]
[Route("table-tennis-matches")]
public class TableTennisMatchesController(ITableTennisMatchService ttMatchService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var result = await ttMatchService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<TableTennisMatchResponse>(
            result.Data.Select(m => m.ToResponse()).ToList(),
            result.TotalCount);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await ttMatchService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TableTennisMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttMatchService.CreateAsync(entity, cancellationToken);
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] TableTennisMatchRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttMatchService.UpdateAsync(id, entity, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await ttMatchService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
