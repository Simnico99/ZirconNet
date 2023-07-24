// <copyright file="IEnvironmentManager.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Enums;

namespace ZirconNet.Core.Environments;

/// <summary>
/// Get important informations about the running environment.
/// </summary>
public interface IEnvironmentManager
{
    /// <summary>
    /// Gets the current environment.
    /// </summary>
    ApplicationEnvironment Environment { get; }

    /// <summary>
    /// Gets a value indicating whether the current environment is debug or not.
    /// </summary>
    bool IsDebug { get; }
}