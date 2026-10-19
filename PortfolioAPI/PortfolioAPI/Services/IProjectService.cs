using PortfolioAPI.DTOs;

namespace PortfolioAPI.Services;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProjectResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}