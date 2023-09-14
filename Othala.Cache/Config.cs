using Microsoft.Extensions.DependencyInjection;

namespace Othala.Cache;

public static class Config
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddTransient<ICacheService, CacheService>();
    }
}