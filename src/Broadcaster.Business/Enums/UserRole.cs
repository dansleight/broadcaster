namespace Broadcaster.Business;

public enum UserRole
{
    Admin,
    User
}

public static class UserRoleExtensions
{
    public static string ToString(this UserRole role)
    {
        return role.ToString().ToUpper();
    }
}