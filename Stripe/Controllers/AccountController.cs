using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Dto;
using Stripe.Services.Interfaces;

namespace Stripe.Controllers;

[ApiController]
[Route("/account")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginDto loginDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.AuthenticateAsync(loginDto, cancellationToken);
        
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegistrationDto registrationDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(registrationDto, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        await _userService.LogoutAsync(userId, cancellationToken);
        
        return Ok();
    }

    [HttpGet("tokens/refresh")]
    public async Task<IActionResult> RefreshAccessTokenAsync(
        [FromQuery] string refreshToken,
        CancellationToken cancellationToken)
    {
        var tokens = await _userService.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        
        return Ok(tokens);
    }
}
