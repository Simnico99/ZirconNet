using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

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
        if (_directoryInfo != null)
        {
            var directoriesQueue = new Queue<DirectoryInfo>();
            directoriesQueue.Enqueue(_directoryInfo);
            while (directoriesQueue.Count > 0)
            {
                var currentDirectory = directoriesQueue.Dequeue();
                var files = currentDirectory.GetFiles();
                foreach (var file in files)
                {
                    var fileWrapper = new FileWrapper(file);
                    CopyingFile.Publish(fileWrapper);
                    await fileWrapper.CopyToDirectoryAsync(destination);
                    CopiedFile.Publish(fileWrapper);
                }
                var subdirectories = currentDirectory.GetDirectories();
                foreach (var subdirectory in subdirectories)
                {
                    directoriesQueue.Enqueue(subdirectory);
                }
            }
        }
    }

    public override void Delete()
    {
        if (_directoryInfo != null && Exists)
        {
            _directoryInfo.Delete(true);
        }
    }

    public IEnumerable<IFileWrapperBase> EnumerateFiles()
    {
        return _directoryInfo.EnumerateFiles().Select(file => new FileWrapper(file, false));
    }

    public IEnumerable<IDirectoryWrapperBase> EnumerateDirectories()
    {
        return _directoryInfo.EnumerateDirectories().Select(directory => new DirectoryWrapper(directory, false));
    }

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
    {
        return _directoryInfo.EnumerateFileSystemInfos();
    }

    public IDirectoryWrapperBase[] GetDirectories()
    {
        return _directoryInfo.GetDirectories().Select(d => new DirectoryWrapper(d, false)).ToArray();
    }

    public IFileWrapperBase[] GetFiles()
    {
        return _directoryInfo.GetFiles().Select(f => new FileWrapper(f, false)).ToArray();
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
