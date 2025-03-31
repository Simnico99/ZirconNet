// <copyright file="ConsoleManager.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.IO;
using ZirconNet.WPF.Interops;

namespace ZirconNet.WPF.Debugging;

public static partial class ConsoleManager
{
    public static bool HasConsole
    {
        get
        {
            return InteropMethods.GetConsoleWindowHandle() != IntPtr.Zero;
        }
    }

    /// <summary>
    /// Creates a new console instance if the process is not attached to a console already.
    /// </summary>
    public static void Show()
    {
        if (!HasConsole)
        {
            InteropMethods.CreateConsole();
            InvalidateOutAndError();
        }
    }

    /// <summary>
    /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
    /// </summary>
    public static void Hide()
    {
        if (HasConsole)
        {
            SetOutAndErrorNull();
            InteropMethods.DestroyConsole();
        }
    }

    public static void Toggle()
    {
        if (HasConsole)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private static void InvalidateOutAndError()
    {
        var type = typeof(Console);
        type?.GetField("_out", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.SetValue(null, null);
        type?.GetField("_error", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.SetValue(null, null);
        type?.GetMethod("InitializeStdOutError", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.Invoke(null, [true]);
    }

    private static void SetOutAndErrorNull()
    {
        Console.SetOut(TextWriter.Null);
        Console.SetError(TextWriter.Null);
    }
}
