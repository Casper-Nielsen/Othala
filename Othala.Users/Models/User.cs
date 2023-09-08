namespace Othala.Users.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserStatus Status { get; set; }
    public List<UserRole> UserRoles { get; set; } = new();

    public User(int id, string name, string email, UserStatus status)
    {
        Id = id;
        Name = name;
        Email = email;
        Status = status;
    }
}