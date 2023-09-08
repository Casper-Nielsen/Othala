using Microsoft.Extensions.DependencyInjection;
using Othala.Users.Repositories;

namespace Othala.Users;

public class Config
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserRoleRepository, UserRoleRepository>();
    }
}