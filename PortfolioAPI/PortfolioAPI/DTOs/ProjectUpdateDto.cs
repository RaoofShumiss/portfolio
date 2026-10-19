using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.DTOs;

public class ProjectUpdateDto
{
    [Required, StringLength(200)]
    public string Titel { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Beschrijving { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Categorie { get; set; } = string.Empty;

    [Required, StringLength(2048), Url]
    public string GitHubUrl { get; set; } = string.Empty;

    [Range(typeof(DateTime), "0001-01-02", "9999-12-31")]
    public DateTime Datum { get; set; }
}