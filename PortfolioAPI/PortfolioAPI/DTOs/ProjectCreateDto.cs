using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.DTOs;

public class ProjectCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public List<string> Technologies { get; set; } = [];
}
