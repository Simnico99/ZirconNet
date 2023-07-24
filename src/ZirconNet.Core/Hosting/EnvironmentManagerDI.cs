﻿// <copyright file="EnvironmentManagerDI.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZirconNet.Core.Enums;
using ZirconNet.Core.Environments;

namespace ZirconNet.Core.Hosting;

/// <inheritdoc cref="IEnvironmentManager"/>/>
internal sealed class EnvironmentManagerDI : IEnvironmentManager
{
    /// <inheritdoc/>
    public ApplicationEnvironment Environment => EnvironmentManager.Current.Environment;

    /// <inheritdoc/>
    public bool IsDebug => EnvironmentManager.Current.IsDebug;
}
