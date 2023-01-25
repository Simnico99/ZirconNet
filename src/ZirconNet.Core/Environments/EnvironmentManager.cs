using System.Linq;
using ZirconNet.Core.Enums;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Environments;

/// <summary>
/// Manage the current state of the environment based of the "DOTNET_" and "DOTNET_DEBUG" enviroment variables.
/// </summary>
public sealed class EnvironmentManager : IEnvironmentManager
{
    /// <summary>
    /// Return the current instance of the <see cref="EnvironmentManager"/>.
    /// </summary>
    public static EnvironmentManager Current { get; }

    /// <summary>
    /// Has only 2 states Development and Production, when running in debug mode it reports Development.
    /// </summary>
    public ApplicationEnvironment Environment { get; private set; }

    /// <summary>
    /// Reports debug when in Debug or Development mode.
    /// </summary>
    public bool IsDebug { get; private set; }

    private EnvironmentManager()
    {
        var currentEnvironment = System.Environment.GetEnvironmentVariable("DOTNET_");
        if (string.IsNullOrWhiteSpace(currentEnvironment))
        {
            System.Environment.SetEnvironmentVariable("DOTNET_", "Production");
            System.Environment.SetEnvironmentVariable("DOTNET_DEBUG", "false");
            Environment = ApplicationEnvironment.Production;
            return;
        }

        if (string.Equals(currentEnvironment, "Development"))
        {
            Environment = ApplicationEnvironment.Development;
            IsDebug = true;
        }
    }

    static EnvironmentManager()
    {
        Current = new EnvironmentManager();
    }

    /// <summary>
    /// Instance the environment from the startup arguments if present.
    /// Uses the switch "-debug" by default.
    /// </summary>
    /// <param name="args">Startup arguments.</param>
    /// <param name="debugArg">The argument used for comparison</param>
    public void SetEnvironmentFromStartupArgs(string[]? args, string debugArg = "-debug")
    {
        if (args?.Length >= 1)
        {
            foreach(var i in 0..args.Length)
            {
                var arg = args[i];
#if NET5_0_OR_GREATER
                if (arg.Contains(debugArg, StringComparison.OrdinalIgnoreCase))
#else
                if (arg.ToUpperInvariant().Contains(debugArg.ToUpperInvariant()))
#endif
                {
                    System.Environment.SetEnvironmentVariable("DOTNET_", "Development");
                    System.Environment.SetEnvironmentVariable("DOTNET_DEBUG", "true");
                    Environment = ApplicationEnvironment.Development;
                }
            }

            if (string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("DOTNET_")))
            {
                System.Environment.SetEnvironmentVariable("DOTNET_", "Production");
                System.Environment.SetEnvironmentVariable("DOTNET_DEBUG", "false");
                Environment = ApplicationEnvironment.Production;
            }
        }
    }
}
