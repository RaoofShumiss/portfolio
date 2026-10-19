using PortfolioAPI.DTOs;

namespace PortfolioAPI.Services;

public interface IBlogService
{
    Task<List<BlogResponseDto>> GetAllAsync();
    Task<BlogResponseDto?> GetByIdAsync(int id);
    Task<BlogResponseDto> CreateAsync(BlogCreateDto dto);
    Task<BlogResponseDto?> UpdateAsync(int id, BlogUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}