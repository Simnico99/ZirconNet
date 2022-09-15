using System.IO.Compression;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class ZipFileWrapper : FileWrapper
{
    public IWeakEvent<string> Extracting { get; } = new WeakEvent<string>();
    public IWeakEvent<string> Extracted { get; } = new WeakEvent<string>();
    public ZipFileWrapper(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public ZipFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }

    public async Task UnzipAsync(string extractionPath)
    {
        using var archive = ZipFile.OpenRead(FullName);
        foreach (var zipArchiveEntry in archive.Entries)
        {
            var zipArchiveEntryNormalizedName = zipArchiveEntry.FullName.Replace(@"/", @"\");

            if (!zipArchiveEntryNormalizedName.EndsWith(@"\", StringComparison.InvariantCulture) && !string.IsNullOrWhiteSpace(zipArchiveEntry.Name))
            {
                var extractionName = $@"\{zipArchiveEntryNormalizedName.Replace(FullName, "")}";
                var extractionPathFullName = extractionPath + $@"\{zipArchiveEntryNormalizedName.Replace(FullName, "")}";

                await Extracting.PublishAsync(extractionName);

                if (File.Exists(extractionPathFullName))
                {
                    File.Delete(extractionPathFullName);
                }

                zipArchiveEntry.ExtractToFile(extractionPathFullName);
                await Extracted.PublishAsync(extractionName);
            }
            else
            {
                var extractionName = $@"\{zipArchiveEntryNormalizedName.Replace(FullName, "")}";
                var extractionPathFullName = extractionPath + extractionName;

                await Extracting.PublishAsync(extractionName);
                Directory.CreateDirectory(extractionPathFullName);
                await Extracted.PublishAsync(extractionName);
            }
        }
        archive.Dispose();
    }

    public int Count()
    {
        using var archive = ZipFile.OpenRead(FullName);
        var count = archive.Entries.Count;
        archive.Dispose();

        return count;
    }
}