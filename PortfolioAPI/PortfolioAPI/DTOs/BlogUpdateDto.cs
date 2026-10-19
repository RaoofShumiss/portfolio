using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.DTOs;

public class BlogUpdateDto
{
    [Required, StringLength(200)]
    public string Titel { get; set; } = string.Empty;

    [Required]
    public string Inhoud { get; set; } = string.Empty;

    [Range(typeof(DateTime), "0001-01-02", "9999-12-31")]
    public DateTime Publicatiedatum { get; set; }
}