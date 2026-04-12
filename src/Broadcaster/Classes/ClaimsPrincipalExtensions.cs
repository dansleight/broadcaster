using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using JasperFx.CommandLine.Descriptions;
using Microsoft.AspNetCore.Authorization;

namespace Broadcaster.Classes;

public static class ClaimsPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal user)
    {
        string? email = user.Claims.SingleOrDefault(c => c.Type == "preferred_username")?.Value;
        email ??= user.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
        if (email == null) throw new UnauthorizedAccessException("Claims missing valid email in `preferred_username` or `email` claims.");
        return email!;
    }

    public static string? GetName(this ClaimsPrincipal user)
        => user.Claims.SingleOrDefault(c => c.Type == "name")?.Value;
}
