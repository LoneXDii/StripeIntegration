using Microsoft.EntityFrameworkCore;
using Stripe.DataAccess.Entities;

namespace Stripe.DataAccess;

public interface IDbContext
{
    public DbSet<UserEntity> Users { get; }
    public DbSet<SubscriptionPlanEntity> SubscriptionPlans { get; }
    public DbSet<PriceEntity> Prices { get;  }
    public DbSet<UserSubscriptionEntity> UserSubscriptions { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
