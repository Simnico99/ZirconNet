namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public abstract class FileWrapperBase : FileSystemInfo, IFileWrapperBase
{
    protected FileInfo _fileInfo;
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

    private static void Copy(string inputFilePath, string outputFilePath)
    {
        const int bufferSize = 1024 * 1024;

        using FileStream fileStream = new(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        using FileStream fs = new(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
        fileStream.SetLength(fs.Length);
        int bytesRead;
        var bytes = new byte[bufferSize];

        while ((bytesRead = fs.Read(bytes, 0, bufferSize)) > 0)
        {
            fileStream.Write(bytes, 0, bytesRead);
        }
    }

    public async Task CopyToDirectoryAsync(IDirectoryWrapperBase directory)
    {
        FileInfo finalPath = new(directory.FullName + @$"\{Name}");
        Delete(finalPath);
        while (IsFileLocked(_fileInfo))
        {
            if (IsFileLocked(_fileInfo))
            {
                await Task.Delay(250);
            }
        }

        Copy(FullName, finalPath.FullName);
    }

    protected static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            stream.Close();
        }
        catch (IOException)
        {
            return true;
        }
        return false;
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
            file.Delete();
        }
    }

    public override void Delete()
    {
        if (Exists)
        {
            _fileInfo.Delete();
        }
    }

    public StreamWriter AppendText()
    {
        return _fileInfo.AppendText();
    }

    public StreamWriter CreateText()
    {
        return _fileInfo.CreateText();
    }

    public StreamReader OpenText()
    {
        return _fileInfo.OpenText();
    }

    public FileStream Open(FileMode fileMode, FileAccess access = default, FileShare share = default)
    {
        return _fileInfo.Open(fileMode, access, share);
    }

    public FileStream OpenRead()
    {
        return _fileInfo.OpenRead();
    }

    public FileStream OpenWrite()
    {
        return _fileInfo.OpenWrite();
    }
}

