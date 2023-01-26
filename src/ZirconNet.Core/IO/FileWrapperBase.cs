namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public abstract class FileWrapperBase : FileSystemInfo, IFileWrapperBase
{
    protected readonly FileInfo _fileInfo;
    public override string FullName => _fileInfo.FullName;
    public override string Name => _fileInfo.Name;
    public override bool Exists => _fileInfo.Exists;

    public FileWrapperBase(string file, bool createFile = true, bool overwrite = false)
    {
        _fileInfo = createFile ? Create(file, overwrite) : new FileInfo(file);
    }
    public FileWrapperBase(FileInfo file, bool createFile = true, bool overwrite = false)
    {
        _fileInfo = createFile ? Create(file.FullName, overwrite) : file;
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

    public static void Copy(string inputFilePath, string outputFilePath)
    {
        File.Copy(inputFilePath, outputFilePath, true);
    }

    public async Task CopyToDirectoryAsync(IDirectoryWrapperBase directory)
    {
        FileInfo finalPath = new(directory.FullName + @$"\{Name}");
        Delete(finalPath);
        while (IsFileLocked(_fileInfo.FullName))
        {
            if (IsFileLocked(_fileInfo.FullName))
            {
                await Task.Delay(250);
            }
        }
        File.Move(_fileInfo.FullName, finalPath.FullName);
    }

    public byte[] GetByteArray()
    {
        var data = File.ReadAllBytes(_fileInfo.FullName);

        return data;
    }

    private static void Delete(FileInfo file)
    {
        if (file.Exists)
        {
            File.Delete(file.FullName);
        }
    }

    public static bool IsFileLocked(string path)
    {
        try
        {
            using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None)) { }
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    public override void Delete()
    {
        if (Exists)
        {
            File.Delete(_fileInfo.FullName);
        }
    }

    public StreamWriter AppendText()
    {
        return File.AppendText(_fileInfo.FullName);
    }

    public StreamWriter CreateText()
    {
        return File.CreateText(_fileInfo.FullName);
    }

    public StreamReader OpenText()
    {
        return File.OpenText(_fileInfo.FullName);
    }

    public FileStream Open(FileMode fileMode, FileAccess access = default, FileShare share = default)
    {
        return File.Open(_fileInfo.FullName, fileMode, access, share);
    }

    public FileStream OpenRead()
    {
        return File.OpenRead(_fileInfo.FullName);
    }

    public FileStream OpenWrite()
    {
        return File.OpenWrite(_fileInfo.FullName);
    }
}