using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stripe.DataAccess.Entities;

namespace Stripe.DataAccess;

public class AppDbContext : IdentityDbContext<UserEntity>, IDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) 
    { }  
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SubscriptionPlanEntity> SubscriptionPlans { get; set; }
    public DbSet<PriceEntity> Prices { get; set; }
    public DbSet<UserSubscriptionEntity> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
