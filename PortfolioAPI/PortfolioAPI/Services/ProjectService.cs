using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

    // Centrale mapping voor queries en opgeslagen entiteiten.
    private static readonly Expression<Func<Project, ProjectResponseDto>> ToResponse = item => new ProjectResponseDto
    {
        Id = item.Id,
        Titel = item.Titel,
        Beschrijving = item.Beschrijving,
        Categorie = item.Categorie,
        GitHubUrl = item.GitHubUrl,
        Datum = item.Datum
    };

    private static readonly Func<Project, ProjectResponseDto> MapToResponse = ToResponse.Compile();

    public Task<List<ProjectResponseDto>> GetAllAsync()
    {
        return _context.Projects.AsNoTracking().OrderBy(item => item.Id)
            .Select(ToResponse).ToListAsync();
    }

    public Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        return _context.Projects.AsNoTracking().Where(item => item.Id == id)
            .Select(ToResponse).SingleOrDefaultAsync();
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        var item = new Project
        {
            Titel = dto.Titel,
            Beschrijving = dto.Beschrijving,
            Categorie = dto.Categorie,
            GitHubUrl = dto.GitHubUrl,
            Datum = dto.Datum
        };
        _context.Projects.Add(item);
        await _context.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var item = await _context.Projects.FindAsync(new object[] { id });
        if (item is null)
        {
            return null;
        }

        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        item.Titel = dto.Titel;
        item.Beschrijving = dto.Beschrijving;
        item.Categorie = dto.Categorie;
        item.GitHubUrl = dto.GitHubUrl;
        item.Datum = dto.Datum;
        await _context.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Projects.FindAsync(new object[] { id });
        if (item is null)
        {
            return false;
        }

        _context.Projects.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}