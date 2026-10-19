namespace PortfolioAPI.Entities;

public class Blog
{
    public int Id { get; set; }

    public string Titel { get; set; } = string.Empty;

    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}