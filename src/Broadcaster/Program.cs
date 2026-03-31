using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Newtonsoft.Json.Converters;
using Broadcaster.Business;
using Broadcaster.Business.Models.Config;
using Broadcaster.Business.Services.RepoBases;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerUI;
using YamlDotNet.Serialization;
using Broadcaster.Stream;

namespace Broadcaster;

public class Program
{
    private static readonly string corsPolicyName = "AllowLocalhost";

    internal static void Main(string[] args)
    {
        LogHelper.UseSerilogBootstrapLogger();
        try
        {
            Log.Logger.Write(LogEventLevel.Information, "Bootstrapping web application");
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilogLogging();
            builder.Host.UseLamar(delegate (HostBuilderContext context, ServiceRegistry registry)
            {
                ConfigureContainer(context, registry);
            });
            WebApplication app = builder.Build();
            Log.Logger.Write(LogEventLevel.Information, "Configuring web application");
            Configure(app, builder.Environment, builder.Configuration);
            Log.Logger.Write(LogEventLevel.Information, "Starting web application");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Logger.Write(LogEventLevel.Fatal, ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static void ConfigureContainer(HostBuilderContext context, ServiceRegistry services)
    {
        // Identity with Microsoft EntraID
        // todo: see if I can have a development api key that doesn't require Entra for development
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddAuthentication("Bearer")
            .AddMicrosoftIdentityWebApi(context.Configuration.GetSection("EntraId"), JwtBearerDefaults.AuthenticationScheme, true)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(context.Configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();

        _ = services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            JwtBearerEvents currentEvents = o.Events;
            IConfiguration configuration = context.Configuration;

            o.Events = new JwtBearerEvents
            {
                OnChallenge = async context => await currentEvents.OnChallenge(context),
                OnAuthenticationFailed = async context => await currentEvents.OnAuthenticationFailed(context),
                OnForbidden = async context => await currentEvents.OnForbidden(context),
                OnTokenValidated = async context =>
                {
                    if (context.Principal == null)
                    {
                        throw new ArgumentNullException("TokenValidatedContext is null and cannot be.");
                    }
                    await currentEvents.OnTokenValidated(context);
                    bool hasEmail = context.Principal?.Claims.Where(x => x.Type == "email").Any() ?? false;
                    bool hasUpn = context.Principal?.Claims.Where(x => x.Type == "upn").Any() ?? false;

                    List<string> roles = RoleMapping.GetRolesFromClaims(configuration, context.Principal!.Claims);
                    List<Claim> claims = context!.Principal!.Claims.ToList();
                    foreach (string role in roles)
                    {
                        claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
                    }

                    context.Principal = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            claims: claims,
                            context!.Principal!.Identity!.AuthenticationType,
                            hasEmail ? "email" : hasUpn ? "upn" : "unique_name",
                            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
                },
                OnMessageReceived = async context =>
                {
                    await currentEvents.OnMessageReceived(context);
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // read the token out of the query string
                        context.Token = accessToken;
                    }
                }
            };
        });

        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        services.AddHttpContextAccessor();

        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    if (origin.StartsWith("https://localhost:") || origin.StartsWith("http://localhost:"))
                        return true;
                    return false;
                });
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });

        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problems = new CustomBadRequest(context);
                    return new BadRequestObjectResult(problems);
                };
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        services.AddSignalR();

        services.Configure<AppSettingsBase>(context.Configuration);
        services.Configure<DataAccessSettings>(context.Configuration);
        services.Configure<BroadcastSettings>(context.Configuration.GetSection("Broadcast"));

        services.AddSpaStaticFiles(opt => opt.RootPath = "ClientApp/dist");

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Broadcaster API", Version = "v1.0", Description = "Broadcaster API Prototype" });
            c.AddJwtBearerSecurity();
            c.IncludeXmlComments(Assembly.GetExecutingAssembly(), true);
            c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");
            c.SupportNonNullableReferenceTypes();
            c.UseAllOfToExtendReferenceSchemas();
            c.OperationFilter<AuthorizeOperationFilter>();
        });
        services.AddSwaggerGenNewtonsoftSupport();

        services.AddMemoryCache();
        services.AddEndpointsApiExplorer();
        services.AddLogging();

        services.Scan(s =>
        {
            s.TheCallingAssembly();
            s.Assembly(Assembly.GetExecutingAssembly());
            s.Assembly("Broadcaster.Business");

            s.WithDefaultConventions(ServiceLifetime.Scoped);

            s.LookForRegistries();
        });

        _ = services.AddSingleton<StreamManager>();
        _ = services.AddSingleton<Assembly[]>([
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(WidgetObject))!
        ]);

        services.AddScoped<BoundTableBinder>();
    }

    public static void Configure(WebApplication app, IWebHostEnvironment env, ConfigurationManager configuration)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
        });

        app.UseSerilogRequestLogging(delegate (RequestLoggingOptions options)
        {
            options.EnrichDiagnosticContext = delegate (IDiagnosticContext diagnosticContext, HttpContext httpContext)
            {
                diagnosticContext.Set("Request", new
                {
                    httpContext.Request.ContentType,
                    httpContext.Request.ContentLength
                }, destructureObjects: true);
                diagnosticContext.Set("Response", new
                {
                    httpContext.Response.ContentType,
                    httpContext.Response.ContentLength
                }, destructureObjects: true);
            };
            options.IncludeQueryInRequestPath = true;
            options.GetLevel = delegate (HttpContext context, double _, Exception? ex)
            {
                if (ex != null || context.Response.StatusCode >= 500)
                {
                    return LogEventLevel.Error;
                }

                if (context.Response.StatusCode >= 400)
                {
                    string text = context.Request.Path;
                    if (!text.Contains("profiler/results") && !text.Contains("favicon.ico"))
                    {
                        return LogEventLevel.Warning;
                    }
                }

                return LogEventLevel.Verbose;
            };
        });

        app.UseCors(corsPolicyName);

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Broadcaster API";
            options.EnableTryItOutByDefault();
            options.EnablePersistAuthorization();
            options.DocExpansion(DocExpansion.List);
            options.InjectJavascript("/swagger-custom.js");
        });

        app.UseMiddleware<NormalizeAuthorizationHeaderMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles();

        // This not is mostly pointless, because our environment is set up to run the API and the SPA separately during development
        // If you are running them both during development, the Node.JS process doesn't stop with the debug session, you'll have to kill it yourself
        app.UseWhen(r => r != null && r.Request != null && r.Request.Path != null && r.Request.Path.Value != null
            && !r.Request.Path.Value.StartsWith("/api")
            && !r.Request.Path.Value.StartsWith("/hub"), builder =>
        {
            builder.UseSpaStaticFiles();
            builder.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });
        });
    }

}