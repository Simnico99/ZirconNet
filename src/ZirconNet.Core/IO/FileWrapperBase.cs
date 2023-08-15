// <copyright file="FileWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

#if NETCOREAPP3_1_OR_GREATER
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Win32.SafeHandles;
#endif

namespace ZirconNet.Core.IO;

/// <inheritdoc cref="IFileWrapperBase"/>
public abstract class FileWrapperBase<T> : BufferedCopyFileSystemInfo, IFileWrapperBase<T>
    where T : FileWrapperBase<T>
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

    public long Length => FileInfo.Length;

    public string? DirectoryName => FileInfo.DirectoryName;

    public IDirectoryWrapperBase? Directory => FileInfo.Directory is not null ? new DirectoryWrapper(FileInfo.Directory, false, false) : null;

    public bool IsReadOnly
    {
        get => FileInfo.IsReadOnly;
        set => FileInfo.IsReadOnly = value;
    }

    protected FileInfo FileInfo { get; }

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

    public T CopyTo(string outputFilePath, bool overwrite = false)
    {
        var newFile = FileInfo.CopyTo(outputFilePath, overwrite);
        return (T)Activator.CreateInstance(typeof(T), newFile, false, false)!;
    }

#if NETCOREAPP3_1_OR_GREATER
    public void MoveTo(string outputFilePath, bool overwrite = false)
    {
        FileInfo.MoveTo(outputFilePath, overwrite);
    }
#else
    public void MoveTo(string outputFilePath)
    {
        FileInfo.MoveTo(outputFilePath);
    }
#endif

    public T Replace(string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors = false)
    {
        var newFile = FileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
        return (T)Activator.CreateInstance(typeof(T), newFile, false, false)!;
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

    public byte[] ReadAllBytes()
    {
        return File.ReadAllBytes(FileInfo.FullName);
    }

    public override void Delete()
    {
        if (Exists)
        {
            FileInfo.Delete();
        }
    }

    public StreamWriter AppendText()
    {
        return FileInfo.AppendText();
    }

    public StreamWriter CreateText()
    {
        return FileInfo.CreateText();
    }

    public StreamReader OpenText()
    {
        return FileInfo.OpenText();
    }

#if NET5_0_OR_GREATER
    public FileStream Open(FileStreamOptions fileStreamOptions)
    {
        return FileInfo.Open(fileStreamOptions);
    }
#endif

    public FileStream Open(FileMode fileMode, FileAccess access = default, FileShare share = default)
    {
        return FileInfo.Open(fileMode, access, share);
    }

    public FileStream OpenRead()
    {
        return FileInfo.OpenRead();
    }

    public FileStream OpenWrite()
    {
        return FileInfo.OpenWrite();
    }

#if NET5_0_OR_GREATER
    public SafeFileHandle OpenHandle(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, FileOptions options = FileOptions.None, long preallocationSize = 0) => File.OpenHandle(FileInfo.FullName, mode, access, share, options, preallocationSize);
#endif

    public void SetCreationTime(DateTime creationTime)
    {
        File.SetCreationTime(FileInfo.FullName, creationTime);
    }

    public void SetCreationTimeUtc(DateTime creationTimeUtc)
    {
        File.SetCreationTime(FileInfo.FullName, creationTimeUtc);
    }

    public DateTime GetCreationTime()
    {
        return File.GetCreationTime(FileInfo.FullName);
    }

    public DateTime GetCreationTimeUtc()
    {
        return File.GetCreationTimeUtc(FileInfo.FullName);
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public void Decrypt() => FileInfo.Decrypt();

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public void Encrypt() => FileInfo.Encrypt();

    public void SetLastAccessTime(DateTime lastAccessTime)
    {
        File.SetLastAccessTime(FileInfo.FullName, lastAccessTime);
    }

    public void SetLastAccessTimeUtc(DateTime lastAccessTimeUtc)
    {
        File.SetLastAccessTimeUtc(FileInfo.FullName, lastAccessTimeUtc);
    }

    public DateTime GetLastAccessTime()
    {
        return File.GetLastAccessTime(FileInfo.FullName);
    }

    public DateTime GetLastAccessTimeUtc()
    {
        return File.GetLastWriteTimeUtc(FileInfo.FullName);
    }

    public void SetLastWriteTime(DateTime lastWriteTime)
    {
        File.SetLastAccessTime(FileInfo.FullName, lastWriteTime);
    }

    public void SetLastWriteTimeUtc(DateTime lastWriteTimeUtc)
    {
        File.SetLastAccessTimeUtc(FileInfo.FullName, lastWriteTimeUtc);
    }

    public DateTime GetLastWriteTime()
    {
        return File.GetLastWriteTime(FileInfo.FullName);
    }

    public DateTime GetLastWriteTimeUtc()
    {
        return File.GetLastWriteTimeUtc(FileInfo.FullName);
    }

    public FileAttributes GetAttributes()
    {
        return File.GetAttributes(FileInfo.FullName);
    }

    public void SetAttributes(FileAttributes fileAttributes)
    {
        File.SetAttributes(FileInfo.FullName, fileAttributes);
    }

#if NET7_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    public UnixFileMode GetUnixFileMode() => File.GetUnixFileMode(FileInfo.FullName);

    [UnsupportedOSPlatform("windows")]
    public void SetUnixFileMode(UnixFileMode mode)
    {
        File.SetUnixFileMode(FileInfo.FullName, mode);
    }
#endif

    public string ReadAllText()
    {
        return File.ReadAllText(FileInfo.FullName);
    }

    public void WriteAllText(string? content)
    {
        File.WriteAllText(FileInfo.FullName, content);
    }

#if NET5_0_OR_GREATER
    public string ReadAllText(Encoding encoding)
    {
        return File.ReadAllText(FileInfo.FullName, encoding);
    }

    public void WriteAllText(string? content, Encoding encoding)
    {
        File.WriteAllText(FileInfo.FullName, content, encoding);
    }
#endif

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