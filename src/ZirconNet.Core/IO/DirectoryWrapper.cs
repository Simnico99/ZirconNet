﻿// <copyright file="DirectoryWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.Versioning;

namespace ZirconNet.Core.IO;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class DirectoryWrapper : DirectoryWrapperBase
{
    public DirectoryWrapper(string directory, bool createDirectory = true, bool overwrite = false)
        : base(directory, createDirectory, overwrite)
    {
    }

    public DirectoryWrapper(DirectoryInfo directory, bool createDirectory = true, bool overwrite = false)
        : base(directory, createDirectory, overwrite)
    {
    }
}
