// <copyright file="ZipFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Buffers;
using System.IO.Compression;
using ZirconNet.Core.Async;
using ZirconNet.Core.Events;
using ZirconNet.Core.Extensions;

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
#if NETCOREAPP3_1_OR_GREATER
            var (normalizedPath, extractionName, extractionPathFullName) = PreparePaths(zipArchiveEntry.FullName, extractionPath);
#else
            var (normalizedPath, extractionName, extractionPathFullName) = PreparePaths(zipArchiveEntry.FullName.AsSpan(), extractionPath.AsSpan());
#endif
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

    private static async Task ExtractFileAsync(string extractionPathFullName, ZipArchiveEntry zipArchiveEntry)
    {
        if (File.Exists(extractionPathFullName))
        {
            using var stream = zipArchiveEntry.Open();
            using var fileStream = new FileStream(extractionPathFullName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            var buffer = ArrayPool<byte>.Shared.Rent(4096);
            try
            {
                int bytesRead;
#if NETCOREAPP3_1_OR_GREATER
                while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) != 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                }
#else
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                }
#endif
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    private async Task ExtractEntryAsync(string normalizedPath, string extractionPathFullName, string extractionName, ZipArchiveEntry zipArchiveEntry)
    {
        Extracting.Publish(extractionName);

        if (normalizedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            if (!Directory.Exists(extractionPathFullName))
            {
                _ = Directory.CreateDirectory(extractionPathFullName);
            }
        }
        else
        {
            await ExtractFileAsync(extractionPathFullName, zipArchiveEntry).ConfigureAwait(false);
        }

        Extracted.Publish(extractionName);
    }

    private (string NormalizedPath, string ExtractionName, string ExtractionPathFullName) PreparePaths(ReadOnlySpan<char> originalPath, ReadOnlySpan<char> extractionPath)
    {
        var normalizedPath = originalPath.Replace(_slash, Path.DirectorySeparatorChar);
        var extractionName = normalizedPath[FullName.Length..];
        var extractionPathFullName = Path.Combine(extractionPath.ToString(), extractionName.ToString());

        return (normalizedPath.ToString(), extractionName.ToString(), extractionPathFullName);
    }
}
