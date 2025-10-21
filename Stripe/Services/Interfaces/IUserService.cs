using Stripe.Dto;

namespace Stripe.Services.Interfaces;

public interface IUserService
{
    Task<TokensDto> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken);
    Task<TokensDto> RegisterAsync(RegistrationDto registrationDto, CancellationToken cancellationToken);
    Task LogoutAsync(string userId, CancellationToken cancellationToken);
    Task<TokensDto> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
