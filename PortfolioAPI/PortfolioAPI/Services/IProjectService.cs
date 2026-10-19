using PortfolioAPI.DTOs;

namespace PortfolioAPI.Services;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}