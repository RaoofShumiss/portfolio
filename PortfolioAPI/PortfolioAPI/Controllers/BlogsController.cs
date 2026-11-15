using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.DTOs;
using PortfolioAPI.Services;

namespace PortfolioAPI.Controllers;

[ApiController]
[Route("api/blogs")]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _service;

    public BlogsController(IBlogService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BlogResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "CanEdit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BlogResponseDto>> Create(BlogCreateDto dto, CancellationToken cancellationToken)
    {
        var item = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEdit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogResponseDto>> Update(int id, BlogUpdateDto dto, CancellationToken cancellationToken)
    {
        var item = await _service.UpdateAsync(id, dto, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanEdit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
