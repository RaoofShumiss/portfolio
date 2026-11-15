using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Data;
using PortfolioAPI.DTOs;

namespace PortfolioAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager, AppDbContext context) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(AccountDto dto)
    {
        // Gebruiker en Gast-rol worden samen opgeslagen, of helemaal niet.
        await using var transaction = await context.Database.BeginTransactionAsync();
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        result = await userManager.AddToRoleAsync(user, "Gast");
        if (!result.Succeeded)
            return Problem("De Gast-rol kon niet worden toegewezen.");

        await transaction.CommitAsync();
        return StatusCode(StatusCodes.Status201Created, new { user.Id, user.Email });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(AccountDto dto)
    {
        var result = await signInManager.PasswordSignInAsync(
            dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized();

        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
}
