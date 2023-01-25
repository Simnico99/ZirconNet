using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZirconNet.Core.Enums;
using ZirconNet.Core.Environments;

namespace ZirconNet.Core.Hosting;
internal sealed class EnvironmentManagerDI : IEnvironmentManager
{
    public ApplicationEnvironment Environment => EnvironmentManager.Current.Environment;

    public bool IsDebug => EnvironmentManager.Current.IsDebug;
}
