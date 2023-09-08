namespace Othala.Users.Repositories.SqlQueries;

internal static class UserRoleQueries
{
    internal const string GetUserRolesFromUserId = @"
SELECT
    Role
FROM UserRoles
WHERE UserId = @UserId;
";

    internal const string AddUserRoles = @"
INSERT INTO UserRoles
    (UserId, Role)
VALUES
    (@UserId, @Role);";
    
    internal const string RemoveUserRoles = @"
REMOVE FROM UserRoles
WHERE UserId = @UserId
    AND Role = @Role;";
}