// <copyright file="DirectoryWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.IO;

public sealed class DirectoryWrapper : DirectoryWrapperBase<DirectoryWrapper>
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
