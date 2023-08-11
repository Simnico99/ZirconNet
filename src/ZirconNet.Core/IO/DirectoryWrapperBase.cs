// <copyright file="DirectoryWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.Versioning;
using System.Security.AccessControl;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;

/// <inheritdoc cref="IDirectoryWrapperBase"/>
public abstract class DirectoryWrapperBase : BufferedCopyFileSystemInfo, IDirectoryWrapperBase
{
    private readonly DirectoryInfo _directoryInfo;

    public DirectoryWrapperBase(string directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory, overwrite) : new DirectoryInfo(directory);
    }

    public DirectoryWrapperBase(DirectoryInfo directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory.FullName, overwrite) : directory;
    }

    public override string FullName => _directoryInfo.FullName;

    public override string Name => _directoryInfo.Name;

    public override bool Exists => _directoryInfo.Exists;

    public IWeakEvent<IFileWrapperBase> CopyingFile { get; } = new WeakEvent<IFileWrapperBase>();

    public IWeakEvent<IFileWrapperBase> CopiedFile { get; } = new WeakEvent<IFileWrapperBase>();

    public async ValueTask CopyContentAsync(IDirectoryWrapperBase destination)
    {
        if (_directoryInfo is not null)
        {
            var directoriesQueue = new Queue<DirectoryInfo>();
            directoriesQueue.Enqueue(_directoryInfo);
            while (directoriesQueue.Count > 0)
            {
                var currentDirectory = directoriesQueue.Dequeue();
                var files = currentDirectory.GetFiles();

                var tasks = files.Select(async file =>
                {
                    var fileWrapper = new FileWrapper(file);
                    CopyingFile.Publish(fileWrapper);
                    await BufferedCopyAsync(fileWrapper, destination).ConfigureAwait(false);
                    CopiedFile.Publish(fileWrapper);
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);

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

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("Windows")]
#endif
    public DirectorySecurity GetAccessControl()
    {
        return _directoryInfo.GetAccessControl();
    }

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("Windows")]
#endif
    public void SetAccessControl(DirectorySecurity directorySecurity)
    {
        _directoryInfo.SetAccessControl(directorySecurity);
    }

    private static DirectoryInfo Create(string path, bool overwrite = false)
    {
        if (Directory.Exists(path) && overwrite)
        {
            new DirectoryInfo(path).Delete(true);
        }

        return Directory.Exists(path) && !overwrite ? new DirectoryInfo(path) : Directory.CreateDirectory(path);
    }
}
