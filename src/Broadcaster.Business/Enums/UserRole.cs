namespace Broadcaster.Business;

public enum UserRole
{
    Admin,
    Tech,
    User
}

public static class UserRoleExtensions
{
    public static string ToString(this UserRole role)
    {
        return role.ToString().ToUpper();
    }
}