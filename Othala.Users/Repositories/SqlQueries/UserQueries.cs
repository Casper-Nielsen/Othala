namespace Othala.Users.Repositories.SqlQueries;

public static class UserQueries
{
    internal const string GetUserById = @"
SELECT
    Id    
    Name,
    Email,
    Status
FROM Users
WHERE
    Id = @Id;
";

    internal const string InsertUser = @"
INSERT INTO Users
( Name, Email, Status) 
VALUES
( @Name, @Email, @Status );";
    
    internal const string GetUserFromLastQuery = @"
SELECT
    Id    
    Name,
    Email,
    Status
FROM Users
WHERE
    Id = LAST_INSERT_ID();
";

    internal const string UpdateUserStatus = @"
UPDATE Users
SET 
    Status = @Status
WHERE
    Id = @Id;
";
}