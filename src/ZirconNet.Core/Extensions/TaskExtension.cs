using Microsoft.Extensions.Logging;

namespace ZirconNet.Core.Extensions;
public static class TaskExtensions
{
    /// <summary>
    /// Observes the task to avoid the UnobservedTaskException event to be raised.
    /// </summary>
    public static void Forget(this Task task, bool printException = false)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task, printException);
        }

        async static Task ForgetAwaited(Task task, bool printException)
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
                logger.LogError(ex, "An exception occured in a fire and forget method:");
            }
        }
    }
}