using Stripe.DataAccess.Entities;
using Stripe.Dto;

namespace Stripe.Services.Interfaces;

public interface ITokenService
{
    TokensDto GetTokens(UserEntity user);
}
