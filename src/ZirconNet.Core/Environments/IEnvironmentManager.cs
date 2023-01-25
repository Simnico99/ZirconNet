using ZirconNet.Core.Enums;

namespace ZirconNet.Core.Environments;
public interface IEnvironmentManager
{
    ApplicationEnvironment Environment { get; }
    bool IsDebug { get; }
}