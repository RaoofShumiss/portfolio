using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Data;
using PortfolioAPI.DTOs;
using PortfolioAPI.Entities;

namespace PortfolioAPI.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    // Map na het inlezen zodat de JSON-lijst als gewone lijst beschikbaar is.
    private static ProjectResponseDto MapToResponse(Project item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        Category = item.Category,
        Technologies = item.Technologies.ToList()
    };

    public async Task<List<ProjectResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.Projects.AsNoTracking().OrderBy(item => item.Id)
            .ToListAsync(cancellationToken);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Projects.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        var item = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Technologies = dto.Technologies.ToList()
        };
        _context.Projects.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponse(item);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (item is null)
        {
            return null;
        }

        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        item.Title = dto.Title;
        item.Description = dto.Description;
        item.Category = dto.Category;
        item.Technologies = dto.Technologies.ToList();
        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _context.Projects.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
