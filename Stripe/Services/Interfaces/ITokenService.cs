using Stripe.DataAccess.Entities;
using Stripe.Dto;

namespace Stripe.Services.Interfaces;

public interface ITokenService
{
    Task<TokensDto> GetTokensAsync(UserEntity user);
    Task<TokensDto?> RefreshAccessTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(UserEntity user);
}
