// <copyright file="ZipFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Buffers;
using System.IO.Compression;
using ZirconNet.Core.Async;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;

public sealed class ZipFileWrapper : FileWrapperBase
{
    private const char _slash = '/';

    private int? _cachedCount;

    public ZipFileWrapper(string file, bool createFile = true, bool overwrite = false)
    : base(file, createFile, overwrite)
    {
    }

    public ZipFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    public IWeakEvent<string> Extracting { get; } = new WeakEvent<string>();

    public IWeakEvent<string> Extracted { get; } = new WeakEvent<string>();

    public async ValueTask UnzipAsync(string extractionPath)
    {
        using var archive = ZipFile.OpenRead(FullName);

        await archive.Entries.ForEach(Environment.ProcessorCount, async (zipArchiveEntry) =>
        {
            var (normalizedPath, extractionName, extractionPathFullName) = PreparePaths(zipArchiveEntry.FullName, extractionPath);
            await ExtractEntryAsync(normalizedPath, extractionPathFullName, extractionName, zipArchiveEntry).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public int Count()
    {
        if (!_cachedCount.HasValue)
        {
            using var archive = ZipFile.OpenRead(FullName);
            _cachedCount = archive.Entries.Count;
        }

        return _cachedCount.Value;
    }

    private async Task ExtractEntryAsync(string normalizedPath, string extractionPathFullName, string extractionName, ZipArchiveEntry zipArchiveEntry)
    {
        Extracting.Publish(extractionName);

        if (normalizedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            CreateDirectoryIfNeeded(extractionPathFullName);
        }
        else
        {
            await ExtractFileAsync(extractionPathFullName, zipArchiveEntry).ConfigureAwait(false);
        }
    }

    private void CreateDirectoryIfNeeded(string extractionPathFullName)
    {
        if (!Directory.Exists(extractionPathFullName))
        {
            _ = Directory.CreateDirectory(extractionPathFullName);
        }
    }

    private async Task ExtractFileAsync(string extractionPathFullName, ZipArchiveEntry zipArchiveEntry)
    {
        if (File.Exists(extractionPathFullName))
        {
            using var stream = zipArchiveEntry.Open();
            using var fileStream = new FileStream(extractionPathFullName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            var buffer = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    private (string NormalizedPath, string ExtractionName, string ExtractionPathFullName) PreparePaths(string originalPath, string extractionPath)
    {
        var normalizedPath = originalPath.Replace(_slash, Path.DirectorySeparatorChar);
        var extractionName = normalizedPath[FullName.Length..];
        var extractionPathFullName = Path.Combine(extractionPath, extractionName);

        return (normalizedPath, extractionName, extractionPathFullName);
    }
}
