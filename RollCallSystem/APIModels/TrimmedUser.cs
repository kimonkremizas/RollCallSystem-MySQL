using RollCallSystem.Database;

namespace RollCallSystem.APIModels;

public class TrimmedUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int? RoleId { get; set; }

    public TrimmedUser() { }
    public TrimmedUser(int id, string email, string firstName, string lastName, int? roleId = null)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        RoleId = roleId;
    }

    public static implicit operator TrimmedUser(User u) => new TrimmedUser { Id = u.Id, Email = u.Email, FirstName = u.FirstName, LastName = u.LastName, RoleId = u.RoleId };
}