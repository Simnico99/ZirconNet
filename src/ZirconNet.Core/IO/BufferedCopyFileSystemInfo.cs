// <copyright file="BufferedCopyFileSystemInfo.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Buffers;

namespace ZirconNet.Core.IO;

public abstract class BufferedCopyFileSystemInfo : FileSystemInfo
{
    private const int _bufferSize = 4096;

    protected static async Task BufferedCopyAsync(string sourcePath, string destinationPath)
    {
        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, _bufferSize, true);
        await BufferedCopyAsync(sourceStream, destinationPath).ConfigureAwait(false);
    }

    protected static async Task BufferedCopyAsync(IFileWrapperBase fileWrapper, IDirectoryWrapperBase destination)
    {
        var destinationPath = Path.Combine(destination.FullName, fileWrapper.Name);

        using var sourceStream = new FileStream(fileWrapper.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: _bufferSize, true);
        await BufferedCopyAsync(sourceStream, destinationPath).ConfigureAwait(false);
    }

    private static async Task BufferedCopyAsync(FileStream sourceStream, string destinationPath)
    {
        using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, _bufferSize, true);

        var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
        try
        {
            int bytesRead;
#if NETCOREAPP3_1_OR_GREATER
            while ((bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, _bufferSize)).ConfigureAwait(false)) > 0)
            {
                await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
            }
#else
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, _bufferSize).ConfigureAwait(false)) > 0)
            {
                await destinationStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
            }
#endif
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
