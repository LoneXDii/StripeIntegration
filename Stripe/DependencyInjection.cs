using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe.Configuration;
using Stripe.DataAccess;
using Stripe.DataAccess.Entities;
using Stripe.Mappers.Implementations;
using Stripe.Mappers.Interfaces;
using Stripe.Services.Implementations;
using Stripe.Services.Interfaces;

namespace Stripe;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        
        services.AddDbContext<IDbContext, AppDbContext>(opt => 
            opt.UseNpgsql(connectionString, opt => opt.EnableRetryOnFailure()));

        services.AddIdentity<UserEntity, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            
        services.Configure<StripeOptions>(options => configuration.GetSection("Stripe").Bind(options))
            .Configure<JwtOptions>(options => configuration.GetSection("Jwt").Bind(options));
        
        StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];
        
        return services;
    }

    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<ITokenService, Services.Implementations.TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubscriptionService, Services.Implementations.SubscriptionService>();
        
        services.AddScoped<Stripe.Checkout.SessionService>();
        services.AddScoped<Stripe.BillingPortal.SessionService>();
        services.AddScoped<CustomerService>();

        services.AddScoped<ISubscriptionPlanMapper, SubscriptionPlanMapper>();
        
        return services;
    }
    
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                });

        return services;
    }
}
