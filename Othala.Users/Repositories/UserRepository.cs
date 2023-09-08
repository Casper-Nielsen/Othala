using Dapper;
using Othala.Shared;
using Othala.Users.Models;
using Othala.Users.Repositories.SqlQueries;
using Microsoft.Extensions.Caching.Memory;

namespace Othala.Users.Repositories;

internal interface IUserRepository
{
    Task<User> GetUser(int id);
    Task<User> AddUser(User user);
    Task UpdateUserStatus(int id, UserStatus status);
}

internal class UserRepository : IUserRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly IMemoryCache _memoryCache;

    public UserRepository(
        IDatabaseConnection databaseConnection,
        IMemoryCache memoryCache)
    {
        _databaseConnection = databaseConnection;
        _memoryCache = memoryCache;
    }

    public async Task<User> GetUser(int id)
    {
        var cacheKey = $"user_{id.ToString()}";
        
        if (_memoryCache.TryGetValue<User>(cacheKey, out var user)) return user!;
        
        using var connection = await _databaseConnection.GetConnection();

        user = await connection.QueryFirstAsync<User>(
            UserQueries.GetUserById,
            new
            {
                id
            });

        _memoryCache.Set(cacheKey, user, TimeSpan.FromMinutes(30));

        return user;
    }

    public async Task<User> AddUser(User user)
    {
        using var connection = await _databaseConnection.GetConnection();

        var userId = await connection.ExecuteScalarAsync<int>(
            UserQueries.InsertUser,
            new
            {
                user.Name,
                user.Email,
                user.Status
            });

        return await GetUser(userId);
    }

    public async Task UpdateUserStatus(int id, UserStatus status)
    {
        using var connection = await _databaseConnection.GetConnection();

        await connection.ExecuteAsync(
            UserQueries.UpdateUserStatus,
            new
            {
                status,
                id
            });
        
        var cache = $"user_{id.ToString()}";
        _memoryCache.Remove(cache);
    }
}