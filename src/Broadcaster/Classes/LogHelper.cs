using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Broadcaster;

public static class LogHelper
{
    public static void UseSerilogBootstrapLogger()
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (Debugger.IsAttached)
        {
            loggerConfiguration.WriteTo.Debug();
        }
        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.WriteTo.Console();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            loggerConfiguration.WriteTo.EventLog("Application", restrictedToMinimumLevel: LogEventLevel.Warning);
        }
        Log.Logger = loggerConfiguration.CreateBootstrapLogger();
    }

    public static IHostBuilder UseSerilogLogging(this IHostBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(HostBuilder));
        }

        return builder.UseSerilog((context, services, configuration) =>
        {
            LoggerConfiguration loggerConfiguration = configuration
                .ReadFrom.Services(services)
                .ReadFrom.Configuration(context.Configuration);

            if (Debugger.IsAttached)
            {
                loggerConfiguration.WriteTo.Debug();
                SelfLog.Enable(msg => Debug.WriteLine(msg));
            }
            else
            {
                SelfLog.Enable(Console.Out);
            }
        });
    }
}
