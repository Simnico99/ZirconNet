// <copyright file="BufferedCopyFileSystemInfo.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.IO;

public abstract class BufferedCopyFileSystemInfo : FileSystemInfo
{
    private const int _bufferSize = 8192;

    protected static async Task BufferedCopyAsync(string sourcePath, string destinationPath)
    {
        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, _bufferSize, true);
        await BufferedCopyAsync(sourceStream, destinationPath);
    }

    protected static async Task BufferedCopyAsync(IFileWrapperBase fileWrapper, IDirectoryWrapperBase destination)
    {
        var destinationPath = Path.Combine(destination.FullName, fileWrapper.Name);

        using var sourceStream = new FileStream(fileWrapper.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: _bufferSize, useAsync: true);
        await BufferedCopyAsync(sourceStream, destinationPath);
    }

    private static async Task BufferedCopyAsync(FileStream sourceStream, string destinationPath)
    {
        using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, _bufferSize, true);

        var buffer = new byte[_bufferSize];
        int bytesRead;
#if NET5_0_OR_GREATER
        while ((bytesRead = await sourceStream.ReadAsync(buffer)) > 0)
        {
            await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead));
        }
#else
        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await destinationStream.WriteAsync(buffer, 0, bytesRead);
        }
#endif
    }
}
