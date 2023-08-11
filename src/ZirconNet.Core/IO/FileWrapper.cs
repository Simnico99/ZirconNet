// <copyright file="FileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.IO;

public sealed class FileWrapper : FileWrapperBase
{
    public FileWrapper(string file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    public FileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }
}