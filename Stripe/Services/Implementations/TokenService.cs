using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe.Configuration;
using Stripe.DataAccess.Entities;
using Stripe.Dto;
using Stripe.Exceptions;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<UserEntity> _userManager;
    
    public TokenService(
        IOptions<JwtOptions> jwtOptions,
        UserManager<UserEntity> userManager)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
    }
    
    public async Task<TokensDto> GetTokensAsync(UserEntity user)
    {
        var accessToken = CreateAccessToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user);
        
        return new TokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokensDto?> RefreshAccessTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        
        if(user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return null;
        }
        
        var accessToken = CreateAccessToken(user);

        return new TokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task RevokeRefreshTokenAsync(UserEntity user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _userManager.UpdateAsync(user);
    }

    private string CreateAccessToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier , user.Id),
            new("StripeId", user.StripeId),
        };
        
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtOptions.AccessTokenExpireMinutes));

        var token = new JwtSecurityToken
        (
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: expires,
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> CreateRefreshTokenAsync(UserEntity user)
    {
        user.RefreshToken = Guid.NewGuid().ToString();
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtOptions.RefreshTokenExpireDays));
        
        await _userManager.UpdateAsync(user);
        
        return user.RefreshToken;
    }
}
