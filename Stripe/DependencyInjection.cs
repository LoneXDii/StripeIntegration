using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe.Checkout;
using Stripe.Configuration;
using Stripe.DataAccess;
using Stripe.DataAccess.Entities;
using Stripe.Services.Implementations;
using Stripe.Services.Interfaces;

namespace Stripe;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        
        services.AddDbContext<AppDbContext>(opt => 
            opt.UseNpgsql(connectionString, opt => opt.EnableRetryOnFailure()));

        services.AddIdentity<UserEntity, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            
        services.Configure<StripeOptions>(options => configuration.GetSection("Stripe").Bind(options))
            .Configure<JwtOptions>(options => configuration.GetSection("Jwt").Bind(options));
        
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        
        return services;
    }

    public static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ITokenService, Services.Implementations.TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubscriptionService, Services.Implementations.SubscriptionService>();
        
        services.AddScoped<SessionService>();
        services.AddScoped<CustomerService>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        return services;
    }
    
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
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
