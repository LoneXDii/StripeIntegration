using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    private readonly IMapper _mapper;

    public UserService(
        SignInManager<UserEntity> signInManager,
        UserManager<UserEntity> userManager,
        CustomerService customerService,
        ITokenService tokenService,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _customerService = customerService;
        _tokenService = tokenService;
        _mapper = mapper;
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
        
        var user = _mapper.Map<UserEntity>(registrationDto);
        
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
}
