using PortfolioAPI.DTOs;
using PortfolioAPI.Services;

namespace PortfolioAPI.Tests;

// Handgemaakte mock zoals MockLogger in de les: geen echte service of database.
public class MockProjectService : IProjectService
{
    public List<ProjectResponseDto> Items { get; set; } = [];
    public ProjectResponseDto? GetByIdResult { get; set; }
    public ProjectResponseDto CreateResult { get; set; } = new();
    public ProjectResponseDto? UpdateResult { get; set; }
    public bool DeleteResult { get; set; }

    public int? ReceivedId { get; private set; }
    public ProjectCreateDto? ReceivedCreateDto { get; private set; }
    public ProjectUpdateDto? ReceivedUpdateDto { get; private set; }
    public int WriteCalls { get; private set; }

    public Task<List<ProjectResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Items);

    public Task<ProjectResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        ReceivedId = id;
        return Task.FromResult(GetByIdResult);
    }

    public Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto, CancellationToken cancellationToken = default)
    {
        ReceivedCreateDto = dto;
        WriteCalls++;
        return Task.FromResult(CreateResult);
    }

    public Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto, CancellationToken cancellationToken = default)
    {
        ReceivedId = id;
        ReceivedUpdateDto = dto;
        WriteCalls++;
        return Task.FromResult(UpdateResult);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        ReceivedId = id;
        WriteCalls++;
        return Task.FromResult(DeleteResult);
    }
}
