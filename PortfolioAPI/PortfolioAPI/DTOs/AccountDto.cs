using System.ComponentModel.DataAnnotations;

namespace PortfolioAPI.DTOs;

public class AccountDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
