using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ReceivingSystem;

public static class ReceivingExtention
{
    public static void AddReceivingDependencies(this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        
    }
}