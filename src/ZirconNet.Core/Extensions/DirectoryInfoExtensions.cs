// <copyright file="DirectoryExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.IO;

namespace ZirconNet.Core.Extensions;

public static class DirectoryInfoExtensions
{
    public static DirectoryWrapper Wrap(this DirectoryInfo directory, bool createDirectory = true, bool overwrite = false)
    {
        return new(directory, createDirectory, overwrite);
    }
}
