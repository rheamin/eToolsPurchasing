#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchasingSystem.BLL;
using PurchasingSystem.DAL;

namespace PurchasingSystem;

public static class PurchasingExtention
{
    public static void AddPurchasingDependencies(this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<PurchasingContext>(options);

        services.AddTransient<PurchasingService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<PurchasingContext>();

            return new PurchasingService(context);
        });

        services.AddTransient<VendorService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<PurchasingContext>();

            return new VendorService(context);
        });

        services.AddTransient<EmployeeService>((ServiceProvider) =>
        {
            var context = ServiceProvider.GetService<PurchasingContext>();

            return new EmployeeService(context);
        });
    }
}