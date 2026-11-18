using PortfolioAPI.DTOs;
using PortfolioAPI.Services;

namespace PortfolioAPI.Tests;

// Handgemaakte mock zoals MockLogger in de les: geen echte service of database.
public class MockBlogService : IBlogService
{
    public List<BlogResponseDto> Items { get; set; } = [];
    public BlogResponseDto? GetByIdResult { get; set; }
    public BlogResponseDto CreateResult { get; set; } = new();
    public BlogResponseDto? UpdateResult { get; set; }
    public bool DeleteResult { get; set; }

    public int? ReceivedId { get; private set; }
    public BlogCreateDto? ReceivedCreateDto { get; private set; }
    public BlogUpdateDto? ReceivedUpdateDto { get; private set; }
    public int WriteCalls { get; private set; }

    public Task<List<BlogResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Items);

    public Task<BlogResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        ReceivedId = id;
        return Task.FromResult(GetByIdResult);
    }

    public Task<BlogResponseDto> CreateAsync(BlogCreateDto dto, CancellationToken cancellationToken = default)
    {
        ReceivedCreateDto = dto;
        WriteCalls++;
        return Task.FromResult(CreateResult);
    }

    public Task<BlogResponseDto?> UpdateAsync(int id, BlogUpdateDto dto, CancellationToken cancellationToken = default)
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
