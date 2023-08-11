// <copyright file="ZipFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.IO.Compression;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;

public sealed class ZipFileWrapper : FileWrapperBase
{
    private const char _slash = '/';

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
        foreach (var zipArchiveEntry in archive.Entries)
        {
            var normalizedPath = zipArchiveEntry.FullName.Replace(_slash, Path.DirectorySeparatorChar);
            var extractionName = normalizedPath[FullName.Length..];
            var extractionPathFullName = Path.Combine(extractionPath, extractionName);

            Extracting.Publish(extractionName);

            if (!normalizedPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.InvariantCulture) && !string.IsNullOrWhiteSpace(zipArchiveEntry.Name))
            {
                using var stream = zipArchiveEntry.Open();
                using var fileStream = File.Create(extractionPathFullName);
                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
            else
            {
                _ = Directory.CreateDirectory(extractionPathFullName);
            }

            Extracted.Publish(extractionName);
        }
    }

    public int Count()
    {
        using var archive = ZipFile.OpenRead(FullName);
        return archive.Entries.Count;
    }
}