// <copyright file="TaskExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using Microsoft.Extensions.Logging;

namespace ZirconNet.Microsoft.DependencyInjection.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Observes the task to avoid the UnobservedTaskException event to be raised.
    /// </summary>
    public static void Forget(this Task task, ILogger logger)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task, logger);
        }

        async static Task ForgetAwaited(Task task, ILogger logger)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred in a fire and forget method:");
            }
        }
    }
}