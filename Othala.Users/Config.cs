using Microsoft.Extensions.DependencyInjection;
using Othala.Users.Repositories;

namespace Othala.Users;

public static class Config
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.Lifetime = ServiceLifetime.Transient);
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserRoleRepository, UserRoleRepository>();
    }
}