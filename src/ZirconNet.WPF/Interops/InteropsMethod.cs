// <copyright file="InteropsMethod.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.InteropServices;
using static ZirconNet.WPF.Interops.InteropValues;

namespace ZirconNet.WPF.Interops;

public sealed partial class InteropMethods
{
    public static void CreateConsole()
    {
        AllocConsole();
    }

    public static void DestroyConsole()
    {
        FreeConsole();
    }

    public static IntPtr GetConsoleWindowHandle()
    {
        return GetConsoleWindow();
    }

#if NET7_0_OR_GREATER
    [LibraryImport(ExternDll.Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport(ExternDll.Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeConsole();

    [LibraryImport(ExternDll.Kernel32)]
    private static partial IntPtr GetConsoleWindow();
#else
    [DllImport(ExternDll.Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [DllImport(ExternDll.Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeConsole();

    [DllImport(ExternDll.Kernel32)]
    private static extern IntPtr GetConsoleWindow();
#endif
}