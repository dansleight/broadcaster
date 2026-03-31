using System.Security.Claims;

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
        List<RoleMapping>? mappings = configuration.GetSection("RoleMappings").Get<List<RoleMapping>>();
        if (mappings == null)
            return new();

        List<string> claimsgroups = claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();

        List<string> roles = new();
        foreach (RoleMapping mapping in mappings)
        {
            foreach (string group in mapping.Groups)
            {
                if (claimsgroups.Contains(group)) roles.Add(mapping.Role);
            }
        }
        return roles.Distinct().ToList();
    }

    #endregion
}

