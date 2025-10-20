using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stripe.Database.Entities;

namespace Stripe.Database;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) 
    { }  
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}