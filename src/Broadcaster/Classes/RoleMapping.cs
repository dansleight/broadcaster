using System.Security.Claims;
using Broadcaster.Business;

namespace Broadcaster;


public class RoleMapping
{
    #region Properties

    public string Role { get; set; } = "User";
    public List<string> Groups { get; set; } = new List<string>();

    #endregion

    #region Static Methods

    public static List<string> GetRolesFromClaims(IConfiguration configuration, IEnumerable<Claim> claims)
    {
        string? email = claims.SingleOrDefault(c => c.Type == "preferred_username")?.Value;
        email ??= claims.SingleOrDefault(c => c.Type == "email")?.Value;
        string name = claims.SingleOrDefault(c => c.Type == "name")?.Value ?? "unknown";

        if (email == null) throw new UnauthorizedAccessException("No usable email claim provided by Identity Provider");

        // UserObject user = UserStaticRepo.GetUserForAuth(configuration, email, name);
        // List<string> rolesAndUnits = user.Roles.Select(r => r.ToString()).ToList();
        // rolesAndUnits.AddRange(user.Units);

        // return rolesAndUnits;
        return [];
    }

    #endregion
}

