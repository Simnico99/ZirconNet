// <copyright file="FileWrapperHelper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.IO;

namespace ZirconNet.Core.Helpers;

public static class FileWrapperHelper
{
    public static IFileWrapperBase WrapUsingFileExtension(FileInfo file)
    {
        var extension = file.Extension.ToLower();
        return extension switch
        {
            ".jar" => new JarFileWrapper(file, false, false),
            ".json" => new JsonFileWrapper(file, false, false),
            ".zip" => new ZipFileWrapper(file, false, false),
            _ => new FileWrapper(file, false, false),
        };
    }
}
