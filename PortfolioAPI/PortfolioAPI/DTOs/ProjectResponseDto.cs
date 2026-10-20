namespace PortfolioAPI.DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<string> Technologies { get; set; } = [];
}
