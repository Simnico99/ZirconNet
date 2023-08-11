// <copyright file="DirectoryWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Security.AccessControl;
using ZirconNet.Core.Async;
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
            await CopyDirectoryContentAsync(_directoryInfo, destination).ConfigureAwait(false);
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

    private async Task CopyDirectoryContentAsync(DirectoryInfo directoryInfo, IDirectoryWrapperBase destination)
    {
        var files = directoryInfo.GetFiles();
        await ParallelAsync.ForEach(files, Environment.ProcessorCount, file => HandleFileAsync(file, destination)).ConfigureAwait(false);

        var subdirectories = directoryInfo.GetDirectories();
        foreach (var subdirectory in subdirectories)
        {
            await CopyDirectoryContentAsync(subdirectory, destination).ConfigureAwait(false);
        }
    }

    private async Task HandleFileAsync(FileInfo file, IDirectoryWrapperBase destination)
    {
        var fileWrapper = new FileWrapper(file);
        CopyingFile.Publish(fileWrapper);
        await BufferedCopyAsync(fileWrapper, destination).ConfigureAwait(false);
        CopiedFile.Publish(fileWrapper);
    }
}
