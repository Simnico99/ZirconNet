// <copyright file="TaskHelper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Helpers;

public static class TaskHelper
{
    public static async Task<T[]> WhenAll<T>(params Task<T>[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            return await allTasks.ConfigureAwait(false);
        }
        catch (Exception)
        {
            // ignore
        }

        throw allTasks.Exception ?? throw new();
    }

    public static async Task WhenAll(params Task[] tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            await allTasks.ConfigureAwait(false);
            return;
        }
        catch (Exception)
        {
            // ignore
        }

        throw allTasks.Exception ?? throw new ();
    }
}
