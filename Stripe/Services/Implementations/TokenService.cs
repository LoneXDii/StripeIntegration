using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Stripe.DataAccess.Entities;
using Stripe.Dto;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public TokensDto GetTokens(UserEntity user)
    {
        var accessToken = CreateAccessToken(user);

        return new TokensDto
        {
            AccessToken = accessToken,
        };
    }

    private string CreateAccessToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier , user.Id)
        };
        
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:AccessExpireMinutes"]));

        var token = new JwtSecurityToken
        (
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: expires,
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
