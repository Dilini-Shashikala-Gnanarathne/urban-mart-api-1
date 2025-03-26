using Microsoft.EntityFrameworkCore;
using urbanMartAPI.BaseEntities;
using urbanMartAPI.SystemEntities;

public class DbContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DbContext GetContext(DbContextType contextType)
    {
        return contextType switch
        {
            DbContextType.Base => _serviceProvider.GetRequiredService<BaseDbContext>(),
            DbContextType.Ecommerce => _serviceProvider.GetRequiredService<EcommerceDbContext>(),
            _ => throw new ArgumentException("Invalid context type")
        };
    }
}
