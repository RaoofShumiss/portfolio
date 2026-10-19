using PortfolioAPI.DTOs;

namespace PortfolioAPI.Services;

public interface IBlogService
{
    Task<List<BlogResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BlogResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BlogResponseDto> CreateAsync(BlogCreateDto dto, CancellationToken cancellationToken = default);
    Task<BlogResponseDto?> UpdateAsync(int id, BlogUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}