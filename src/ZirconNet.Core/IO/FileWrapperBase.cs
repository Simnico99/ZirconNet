// <copyright file="FileWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.Versioning;

namespace ZirconNet.Core.IO;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif

/// <inheritdoc cref="IFileWrapperBase"/>
public abstract class FileWrapperBase : FileSystemInfo, IFileWrapperBase
{
    public FileWrapperBase(string file, bool createFile = true, bool overwrite = false)
    {
        FileInfo = createFile ? Create(file, overwrite) : new FileInfo(file);
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
            // Test if file locked
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
        FileInfo finalPath = new(directory.FullName + @$"\{Name}");
        Delete(finalPath);
        while (IsFileLocked(FileInfo.FullName))
        {
            if (IsFileLocked(FileInfo.FullName))
            {
                await Task.Delay(250).ConfigureAwait(false);
            }
        }

        File.Move(FileInfo.FullName, finalPath.FullName);
    }

    public byte[] GetByteArray()
    {
        var data = File.ReadAllBytes(FileInfo.FullName);

        return data;
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
        if (File.Exists(path) && overwrite)
        {
            new FileInfo(path).Delete();
        }

        if (File.Exists(path) && !overwrite)
        {
            return new FileInfo(path);
        }

        FileInfo fileInfo = new(path);
        fileInfo.Create().Dispose();

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