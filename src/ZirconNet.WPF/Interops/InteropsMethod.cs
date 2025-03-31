// <copyright file="InteropsMethod.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.InteropServices;
using static ZirconNet.WPF.Interops.InteropValues;

namespace ZirconNet.WPF.Interops;

public sealed partial class InteropMethods
{
    internal static void CreateConsole()
    {
        AllocConsole();
    }

#if NET7_0_OR_GREATER
    [LibraryImport(ExternDll.Kernel32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial void AllocConsole();
#else
    [DllImport(ExternDll.Kernel32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void AllocConsole();
#endif
}