using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Data;
using PortfolioAPI.DTOs;
using PortfolioAPI.Entities;

namespace PortfolioAPI.Services;

public class BlogService : IBlogService
{
    private readonly AppDbContext _context;

    public BlogService(AppDbContext context)
    {
        _context = context;
    }

    // Centrale mapping voor queries en opgeslagen entiteiten.
    private static readonly Expression<Func<Blog, BlogResponseDto>> ToResponse = item => new BlogResponseDto
    {
        Id = item.Id,
        Titel = item.Titel,
        Inhoud = item.Inhoud,
        Publicatiedatum = item.Publicatiedatum
    };

    private static readonly Func<Blog, BlogResponseDto> MapToResponse = ToResponse.Compile();

    public Task<List<BlogResponseDto>> GetAllAsync()
    {
        return _context.Blogs.AsNoTracking().OrderBy(item => item.Id)
            .Select(ToResponse).ToListAsync();
    }

    public Task<BlogResponseDto?> GetByIdAsync(int id)
    {
        return _context.Blogs.AsNoTracking().Where(item => item.Id == id)
            .Select(ToResponse).SingleOrDefaultAsync();
    }

    public async Task<BlogResponseDto> CreateAsync(BlogCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        var item = new Blog
        {
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };
        _context.Blogs.Add(item);
        await _context.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task<BlogResponseDto?> UpdateAsync(int id, BlogUpdateDto dto)
    {
        var item = await _context.Blogs.FindAsync(new object[] { id });
        if (item is null)
        {
            return null;
        }

        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        item.Titel = dto.Titel;
        item.Inhoud = dto.Inhoud;
        item.Publicatiedatum = dto.Publicatiedatum;
        await _context.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Blogs.FindAsync(new object[] { id });
        if (item is null)
        {
            return false;
        }

        _context.Blogs.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}