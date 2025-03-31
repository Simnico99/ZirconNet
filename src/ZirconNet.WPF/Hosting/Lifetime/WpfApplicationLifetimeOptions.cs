// <copyright file="WpfApplicationLifetimeOptions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.WPF.Hosting.Lifetime;

public sealed class WpfApplicationLifetimeOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether indicates if host lifetime status messages should be suppressed such as on startup.
    /// The default is false.
    /// </summary>
    public bool SuppressStatusMessages
    {
        get; set;
    }
}
