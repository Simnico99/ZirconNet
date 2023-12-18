// <copyright file="TaskExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Diagnostics;
using System.Net.NetworkInformation;

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
                PrintException(ex, printException);
            }
        }
    }

    [Conditional("DEBUG")]
    private static void PrintException(Exception ex, bool printException)
    {
        if (printException)
        {
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
        }
    }
}