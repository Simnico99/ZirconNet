// <copyright file="FileWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.IO;

/// <inheritdoc cref="IFileWrapperBase"/>
public abstract class FileWrapperBase : BufferedCopyFileSystemInfo, IFileWrapperBase
{
    public FileWrapperBase(string file, bool createFile = true, bool overwrite = false)
    {
        FileInfo = createFile ? Create(file, overwrite) : new(file);
    }

    public FileWrapperBase(FileInfo file, bool createFile = true, bool overwrite = false)
    {
        FileInfo = createFile ? Create(file.FullName, overwrite) : file;
    }

    public override string FullName => FileInfo.FullName;

    public override string Name => FileInfo.Name;

    public override bool Exists => FileInfo.Exists;

    protected FileInfo FileInfo { get; }

    public static void Copy(string inputFilePath, string outputFilePath)
    {
        File.Copy(inputFilePath, outputFilePath, true);
    }

    public static bool IsFileLocked(string path)
    {
        try
        {
            using var discard = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    public async ValueTask CopyToDirectoryAsync(IDirectoryWrapperBase directory)
    {
        var finalPath = Path.Combine(directory.FullName, Name);
        Delete(new FileInfo(finalPath));
        while (IsFileLocked(FileInfo.FullName))
        {
            await Task.Delay(100).ConfigureAwait(false);
        }

        await BufferedCopyAsync(FileInfo.FullName, finalPath).ConfigureAwait(false);
    }

    public byte[] GetByteArray()
    {
        return File.ReadAllBytes(FileInfo.FullName);
    }

    public override void Delete()
    {
        if (Exists)
        {
            File.Delete(FileInfo.FullName);
        }
    }

    public StreamWriter AppendText()
    {
        return File.AppendText(FileInfo.FullName);
    }

    public StreamWriter CreateText()
    {
        return File.CreateText(FileInfo.FullName);
    }

    public StreamReader OpenText()
    {
        return File.OpenText(FileInfo.FullName);
    }

    public FileStream Open(FileMode fileMode, FileAccess access = default, FileShare share = default)
    {
        return File.Open(FileInfo.FullName, fileMode, access, share);
    }

    public FileStream OpenRead()
    {
        return File.OpenRead(FileInfo.FullName);
    }

    public FileStream OpenWrite()
    {
        return File.OpenWrite(FileInfo.FullName);
    }

    private static FileInfo Create(string path, bool overwrite = false)
    {
        if (File.Exists(path))
        {
            if (overwrite)
            {
                File.Delete(path);
            }

            return new FileInfo(path);
        }

        var fileInfo = new FileInfo(path);
        using var discard = fileInfo.Create();
        return fileInfo;
    }

    private static void Delete(FileInfo file)
    {
        if (file.Exists)
        {
            File.Delete(file.FullName);
        }
    }
}