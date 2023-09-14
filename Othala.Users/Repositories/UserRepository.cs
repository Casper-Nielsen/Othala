using Dapper;
using Othala.Cache;
using Othala.Shared;
using Othala.Users.Models;
using Othala.Users.Repositories.SqlQueries;

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
    private readonly ICacheService _cacheService;

    public UserRepository(
        IDatabaseConnection databaseConnection,
        ICacheService cacheService)
    {
        _databaseConnection = databaseConnection;
        _cacheService = cacheService;
    }

    public async Task<User> GetUser(int id)
    {
        return await _cacheService.GetOrCreate(CacheConstants.User, id.ToString(), async () =>
            {
                using var connection = await _databaseConnection.GetConnection();

                return await connection.QueryFirstAsync<User>(
                    UserQueries.GetUserById,
                    new
                    {
                        id
                    });
            }
        );
    }

    public async Task<User> AddUser(User user)
    {
        using var connection = await _databaseConnection.GetConnection();

        var insertedUser = await connection.ExecuteScalarAsync<User>(
            UserQueries.InsertUser + UserQueries.GetUserFromLastQuery,
            new
            {
                user.Name,
                user.Email,
                user.Status
            });

        _cacheService.Remove(CacheConstants.User);
        _cacheService.Set(CacheConstants.User, insertedUser.Id.ToString(), insertedUser);

        return insertedUser;
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

        _cacheService.Remove(CacheConstants.User, id.ToString());
    }
}