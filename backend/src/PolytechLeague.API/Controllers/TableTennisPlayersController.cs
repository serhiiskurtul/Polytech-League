using PolytechLeague.Application.Dto.TableTennisDto;
using PolytechLeague.Application.Services.Interfaces.TableTennis;
using PolytechLeague.Domain.Params;
using PolytechLeague.Domain.Entities.TableTennis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolytechLeague.API.Controllers;

[ApiController]
[Route("table-tennis-players")]
public class TableTennisPlayersController(ITableTennisPlayerService ttPlayerService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FilterParams? filterParams,
        [FromQuery] SortParams? sortParams,
        [FromQuery] PageParams? pageParams,
        CancellationToken cancellationToken)
    {
        var result = await ttPlayerService.GetPageAsync(filterParams, sortParams, pageParams, cancellationToken);

        var response = new PageResult<TableTennisPlayerResponse>(
            result.Data.Select(p => p.ToResponse()).ToList(),
            result.TotalCount);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await ttPlayerService.GetByIdAsync(id, cancellationToken);
        return Ok(entity.ToResponse());
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] TableTennisPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttPlayerService.CreateAsync(entity, cancellationToken);
        return Created();
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] TableTennisPlayerRequest request,
        CancellationToken cancellationToken)
    {
        var entity = request.FromRequest();
        await ttPlayerService.UpdateAsync(id, entity, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        await ttPlayerService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
