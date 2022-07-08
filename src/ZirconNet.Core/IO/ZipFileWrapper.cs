﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;
public class ZipFileWrapperinternal : FileWrapper
{
    public IWeakEvent<string> Extracting { get; } = new WeakEvent<string>();
    public IWeakEvent<string> Extracted { get; } = new WeakEvent<string>();


    public ZipFileWrapperinternal(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public ZipFileWrapperinternal(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }

    public async Task UnzipAsync(DirectoryInfo extractionPath)
    {

        using var archive = ZipFile.OpenRead(FullName);
        foreach (var zipArchiveEntry in archive.Entries)
        {
            var zipArchiveEntryNormalizedName = zipArchiveEntry.FullName.Replace(@"/", @"\");

            if (!zipArchiveEntryNormalizedName.EndsWith(@"\", StringComparison.InvariantCulture) && !string.IsNullOrWhiteSpace(zipArchiveEntry.Name))
            {
                var extractionName = $@"\{zipArchiveEntryNormalizedName.Replace(FullName, "")}";
                var extractionPathFullName = extractionPath.FullName + $@"\{zipArchiveEntryNormalizedName.Replace(FullName, "")}";

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
                var extractionPathFullName = extractionPath.FullName + extractionName;

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

