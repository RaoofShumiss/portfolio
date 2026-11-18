using PortfolioAPI.DTOs;
using PortfolioAPI.Services;

namespace PortfolioAPI.Tests;

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

    public Task<List<ProjectResponseDto>> GetAllAsync()
        => Task.FromResult(Items);

    public Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        ReceivedId = id;
        return Task.FromResult(GetByIdResult);
    }

    public Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto)
    {
        ReceivedCreateDto = dto;
        WriteCalls++;
        return Task.FromResult(CreateResult);
    }

    public Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        ReceivedId = id;
        ReceivedUpdateDto = dto;
        WriteCalls++;
        return Task.FromResult(UpdateResult);
    }

    public Task<bool> DeleteAsync(int id)
    {
        ReceivedId = id;
        WriteCalls++;
        return Task.FromResult(DeleteResult);
    }
}
