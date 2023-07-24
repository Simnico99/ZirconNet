// <copyright file="ValueTaskExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Logging;

namespace ZirconNet.Core.Extensions;

public static class ValueTaskExtensions
{
    /// <summary>
    /// Observes the task to avoid the UnobservedTaskException event to be raised.
    /// </summary>
    public static void Forget(this ValueTask task, bool printException = false)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task, printException);
        }

        async static Task ForgetAwaited(ValueTask task, bool printException)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (printException)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    /// <summary>
    /// Observes the task to avoid the UnobservedTaskException event to be raised.
    /// </summary>
    public static void Forget(this ValueTask task, ILogger logger)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task, logger);
        }

        async static Task ForgetAwaited(ValueTask task, ILogger logger)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occured in a fire and forget method:");
            }
        }
    }
}
