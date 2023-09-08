using System.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabaseConnection _databaseConnection;

    public UserRoleRepository(
        IMemoryCache memoryCache, 
        IDatabaseConnection databaseConnection)
    {
        _memoryCache = memoryCache;
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<UserRole>> GetUserRoles(int userId, bool forceRefresh = false)
    {
        var cacheKey = $"user-roles_{userId.ToString()}";

        if (!forceRefresh && _memoryCache.TryGetValue<IEnumerable<UserRole>>(cacheKey, out var userRoles)) return userRoles!;
        
        using var connection = await _databaseConnection.GetConnection();
        
        userRoles = await connection.QueryAsync<UserRole>(
            UserRoleQueries.GetUserRolesFromUserId,
            new
            {
                userId
            });
        
        _memoryCache.Set(cacheKey, userRoles, TimeSpan.FromMinutes(30));
        
        return userRoles;
        
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
        
        _memoryCache.Remove($"user-roles_{userId.ToString()}");
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
        
        _memoryCache.Remove($"user-roles_{userId.ToString()}");
    }
}