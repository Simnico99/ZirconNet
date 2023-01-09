using System.Security.AccessControl;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public abstract class DirectoryWrapperBase : FileSystemInfo, IDirectoryWrapperBase
{
    private readonly DirectoryInfo _directoryInfo;
    public override string FullName => _directoryInfo.FullName;
    public override string Name => _directoryInfo.Name;
    public override bool Exists => _directoryInfo.Exists;
    public IWeakEvent<IFileWrapperBase> CopyingFile { get; } = new WeakEvent<IFileWrapperBase>();
    public IWeakEvent<IFileWrapperBase> CopiedFile { get; } = new WeakEvent<IFileWrapperBase>();

    public DirectoryWrapperBase(string directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory, overwrite) : new DirectoryInfo(directory);
    }

    public DirectoryWrapperBase(DirectoryInfo directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory.FullName, overwrite) : directory;
    }

    private static DirectoryInfo Create(string path, bool overwrite = false)
    {
        if (Directory.Exists(path) && overwrite)
        {
            new DirectoryInfo(path).Delete(true);
        }

        return Directory.Exists(path) && !overwrite ? new DirectoryInfo(path) : Directory.CreateDirectory(path);
    }

    public async Task CopyContentAsync(IDirectoryWrapperBase destination)
    {
        if (_directoryInfo is not null)
        {
            foreach (var file in _directoryInfo.GetFiles())
            {
                var fileWrapper = new FileWrapper(file);
                await CopyingFile.PublishAsync(fileWrapper);
                await fileWrapper.CopyToDirectoryAsync(destination);
                await CopiedFile.PublishAsync(fileWrapper);
            }

            foreach (var folder in _directoryInfo.GetDirectories())
            {
                var folderModel = new DirectoryWrapper(folder, false);

                using (folderModel.CopyingFile.Subscribe((x) => CopyingFile.PublishAsync(x)))
                using (folderModel.CopiedFile.Subscribe((x) => CopiedFile.PublishAsync(x)))
                {
                    await folderModel.CopyContentAsync(destination);
                }
            }
        }
    }

    public override void Delete()
    {
        if (_directoryInfo is not null && Exists)
        {
            _directoryInfo.Delete(true);
        }
    }

    public IEnumerable<IFileWrapperBase> EnumerateFiles()
    {
        foreach (var file in _directoryInfo.EnumerateFiles())
        {
            yield return new FileWrapper(file, false);
        }
    }

    public IEnumerable<IDirectoryWrapperBase> EnumerateDirectories()
    {
        foreach (var directory in _directoryInfo.EnumerateDirectories())
        {
            yield return new DirectoryWrapper(directory, false);
        }
    }

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
    {
        return _directoryInfo.EnumerateFileSystemInfos();
    }

    public IDirectoryWrapperBase[] GetDirectories()
    {
        var directoryWrappers = new DirectoryWrapper[_directoryInfo.GetDirectories().Length];
        var directoryInfos = _directoryInfo.GetDirectories();
        for (var i = 0; i < directoryInfos.Length; i++)
        {
            var directoryInfo = directoryInfos[i];
            directoryWrappers[i] = new DirectoryWrapper(directoryInfo, false);
        }
        return directoryWrappers;
    }

    public IFileWrapperBase[] GetFiles()
    {
        var fileWrappers = new FileWrapper[_directoryInfo.GetFiles().Length];
        var filedInfos = _directoryInfo.GetFiles();
        for (var i = 0; i < filedInfos.Length; i++)
        {
            var fileInfo = filedInfos[i];
            fileWrappers[i] = new FileWrapper(fileInfo, false);
        }
        return fileWrappers;
    }

    public DirectorySecurity GetAccessControl()
    {
        return _directoryInfo.GetAccessControl();
    }

    public void SetAccessControl(DirectorySecurity directorySecurity)
    {
        _directoryInfo.SetAccessControl(directorySecurity);
    }
}
