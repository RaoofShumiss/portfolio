namespace PortfolioAPI.DTOs;

public class BlogResponseDto
{
    public int Id { get; set; }

    public string Titel { get; set; } = string.Empty;

    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}