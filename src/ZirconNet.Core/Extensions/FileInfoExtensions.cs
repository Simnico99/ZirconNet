// <copyright file="FileInfoExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.IO;

namespace ZirconNet.Core.Extensions;

public static class FileInfoExtensions
{
    public static FileWrapper Wrap(this FileInfo file, bool createFile = true, bool overwrite = false)
    {
        return new(file, createFile, overwrite);
    }

    public static JsonFileWrapper WrapAsJson(this FileInfo file, bool createFile = true, bool overwrite = false)
    {
        return new(file, createFile, overwrite);
    }

    public static JarFileWrapper WrapAsJar(this FileInfo file, bool createFile = true, bool overwrite = false)
    {
        return new(file, createFile, overwrite);
    }

    public static ZipFileWrapper WrapAsZip(this FileInfo file, bool createFile = true, bool overwrite = false)
    {
        return new(file, createFile, overwrite);
    }
}
