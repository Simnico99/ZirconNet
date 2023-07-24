// <copyright file="WindowExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Windows;

namespace ZirconNet.WPF.Extensions;

public static class WindowExtension
{
    /// <summary>
    /// Show a dialog asyncronously. (Prevents the main thread from blocking when showing a new window.)
    /// </summary>
    /// <param name="self">The window itself.</param>
    /// <returns>A task waiting for the window to close.</returns>
    public static Task<bool?> ShowDialogAsync(this Window self)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(self);
#else
        if (self == null)
        {
            throw new ArgumentNullException(nameof(self));
        }
#endif
        var completion = new TaskCompletionSource<bool?>();
        self.Dispatcher.BeginInvoke(new Action(() => completion.SetResult(self.ShowDialog())));

        return completion.Task;
    }
}
