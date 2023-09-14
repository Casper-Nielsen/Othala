using System.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Othala.Cache;
using Othala.Shared;
using Othala.Users.Models;
using Othala.Users.Repositories.SqlQueries;

namespace Othala.Users.Repositories;

internal interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetUserRoles(int userId, bool forceRefresh = false);
    Task AddUserRole(int userId, UserRole userRoles);
    Task RemoveUserRole(int userId, UserRole userRoles);
}

internal class UserRoleRepository : IUserRoleRepository
{
    private readonly ICacheService _cacheService;
    private readonly IDatabaseConnection _databaseConnection;

    public UserRoleRepository(
        ICacheService cacheService,
        IDatabaseConnection databaseConnection)
    {
        _cacheService = cacheService;
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<UserRole>> GetUserRoles(int userId, bool forceRefresh = false)
    {
        return await _cacheService.GetOrCreate(CacheConstants.UserRole, userId.ToString(), async () =>
        {
            using var connection = await _databaseConnection.GetConnection();

            var userRoles = await connection.QueryAsync<UserRole>(
                UserRoleQueries.GetUserRolesFromUserId,
                new
                {
                    userId
                });

            return userRoles;
        });
    }

    public async Task AddUserRole(int userId, UserRole userRoles)
    {
        using var connection = await _databaseConnection.GetConnection();

        await connection.ExecuteAsync(
            UserRoleQueries.AddUserRoles,
            new
            {
                userId,
                userRoles
            });

        _cacheService.Remove(CacheConstants.UserRole, userId.ToString());
    }

    public async Task RemoveUserRole(int userId, UserRole userRoles)
    {
        using var connection = await _databaseConnection.GetConnection();
        
        await connection.ExecuteAsync(
            UserRoleQueries.RemoveUserRoles,
            new
            {
                userId,
                userRoles
            });
        
        _cacheService.Remove(CacheConstants.UserRole, userId.ToString());
    }
}