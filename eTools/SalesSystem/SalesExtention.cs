using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesSystem.BLL;
using SalesSystem.DAL;

namespace SalesSystem;

public static class SalesExtention
{
    public static void AddSalesDependencies(this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<SalesContext>(options);
        services.AddTransient<SalesService>();
    }
}