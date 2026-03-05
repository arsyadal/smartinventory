using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Application.Interfaces.Services;
using SmartInventory.Infrastructure.BackgroundServices;
using SmartInventory.Infrastructure.Persistence;
using SmartInventory.Infrastructure.Persistence.Seeders;
using SmartInventory.Infrastructure.Repositories;
using SmartInventory.Infrastructure.Scrapers;

namespace SmartInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SmartInventory.Infrastructure")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IStockMutationRepository, StockMutationRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Scraper strategy — use mock by default (safe for portfolio)
        var useMock = configuration.GetValue<bool>("ScraperSettings:UseMockScraper", true);
        if (useMock)
            services.AddScoped<IScraperStrategy, MockMarketplaceScraper>();
        else
            services.AddScoped<IScraperStrategy, SeleniumScraper>();

        services.AddScoped<IScraperService, ScraperService>();

        // Background service
        services.AddHostedService<PriceScraperHostedService>();

        // Seeder
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
