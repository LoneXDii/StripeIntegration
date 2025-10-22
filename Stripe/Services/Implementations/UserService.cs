using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe.DataAccess;
using Stripe.DataAccess.Entities;
using Stripe.Dto;
using Stripe.Exceptions;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class UserService : IUserService
{
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly UserManager<UserEntity> _userManager;
    private readonly CustomerService _customerService;
    private readonly ITokenService _tokenService;
    private readonly IDbContext _dbContext;

    public UserService(
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager,
        CustomerService customerService,
        ITokenService tokenService,
        IDbContext dbContext)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _customerService = customerService;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }
    
    public async Task<TokensDto> AuthenticateAsync(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(
            loginDto.Email,
            loginDto.Password,
            false,
            false);

        if (!signInResult.Succeeded)
        {
            throw new BadRequestException("Invalid username or password.");
        }
        
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        var tokens = await _tokenService.GetTokensAsync(user);

        return tokens;
    }

    public async Task<TokensDto> RegisterAsync(RegistrationDto registrationDto, CancellationToken cancellationToken)
    {
        if (registrationDto.Password != registrationDto.PasswordConfirmation)
        {
            throw new BadRequestException("Passwords do not match.");
        }

        var user = new UserEntity
        {
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            Email = registrationDto.Email,
            UserName = registrationDto.Email,
        };
        
        var customerOptions = new CustomerCreateOptions
        {
            Email = registrationDto.Email,
        };

        var customer = await _customerService.CreateAsync(customerOptions, cancellationToken: cancellationToken);
        
        user.StripeId = customer.Id;
        
        var registrationResult = await _userManager.CreateAsync(user, registrationDto.Password);

        if (!registrationResult.Succeeded)
        {
            throw new BadRequestException("Invalid credentials.");
        }
        
        var tokens = await _tokenService.GetTokensAsync(user);
        
        return tokens;
    }

    public async Task LogoutAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException("User not exists.");
        }
        
        await _tokenService.RevokeRefreshTokenAsync(user);
    }

    public async Task<TokensDto> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokens = await _tokenService.RefreshAccessTokenAsync(refreshToken);

        if (tokens is null)
        {
            throw new BadRequestException("Invalid refresh token.");
        }
        
        return tokens;
    }

    public async Task<UserSubscriptionDto> GetUserSubscriptionAsync(string userId, CancellationToken cancellationToken)
    {
        var userSubscription = await _dbContext.UserSubscriptions
            .Include(us => us.Price)
            .ThenInclude(p => p.SubscriptionPlan)
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

        if (userSubscription is null)
        {
            throw new NotFoundException("Specified user does not have a subscription.");
        }

        return new UserSubscriptionDto
        {
            SubscriptionPlanName = userSubscription.Price.SubscriptionPlan.Name,
            Price = userSubscription.Price.Price,
            Currency = userSubscription.Price.Currency,
            BillingPeriod = userSubscription.Price.BillingPeriod,
            Status = userSubscription.SubscriptionStatus,
            StartDateTimeUtc = userSubscription.StartDateTimeUtc,
            EndDateTimeUtc = userSubscription.EndDateTimeUtc,
            PeriodEndDateTimeUtc = userSubscription.PeriodEndDateTimeUtc
        };
    }
}
