using System;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Broadcaster;

public static class AuthenticationStartupExtensions
{
    public static void AddJwtBearerSecurity(this SwaggerGenOptions options)
    {
        OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Paste in your JWT token."
        };

        options.AddSecurityDefinition("Bearer", securityScheme);
    }
}

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint has [Authorize] on the method or controller
        var hasAuthorize =
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
            context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            // Add security requirement for endpoints with [Authorize]
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", context.Document),
                        new List<string>()
                    }
                }
            };
        }
    }
}
