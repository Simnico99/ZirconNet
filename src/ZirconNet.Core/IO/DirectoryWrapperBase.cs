// <copyright file="DirectoryWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Security.AccessControl;
using ZirconNet.Core.Async;
using ZirconNet.Core.Events;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.IO;

/// <inheritdoc cref="IDirectoryWrapperBase"/>
public abstract class DirectoryWrapperBase<T> : BufferedCopyFileSystemInfo, IDirectoryWrapperBase<T>
    where T : DirectoryWrapperBase<T>
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

    public T? Parent => _directoryInfo.Parent is null ? null : (T)Activator.CreateInstance(typeof(T), _directoryInfo.Parent, false, false)!;

    public T Root => (T)Activator.CreateInstance(typeof(T), _directoryInfo.Root, false, false)!;

    public IWeakEvent<IFileWrapperBase> CopyingFile { get; } = new WeakEvent<IFileWrapperBase>(true);

    public IWeakEvent<IFileWrapperBase> CopiedFile { get; } = new WeakEvent<IFileWrapperBase>(true);

    public async ValueTask CopyContentAsync(IDirectoryWrapperBase destination)
    {
        if (_directoryInfo is not null)
        {
            await CopyDirectoryContentAsync(_directoryInfo, destination).ConfigureAwait(false);
        }
    }

    public override void Delete()
    {
        Delete(false);
    }

    public void Delete(bool recursive)
    {
        if (_directoryInfo != null && Exists)
        {
            _directoryInfo.Delete(recursive);
        }
    }

    public void MoveTo(string destDirName)
    {
        _directoryInfo.MoveTo(destDirName);
    }

    public IEnumerable<IFileWrapperBase> EnumerateFiles()
    {
        foreach (var file in _directoryInfo.EnumerateFiles())
        {
            yield return file.AutoWrapByExtension(false, false);
        }
    }

    public IEnumerable<IFileWrapperBase> EnumerateFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        foreach (var file in _directoryInfo.EnumerateFiles(searchPattern, searchOption))
        {
            yield return file.AutoWrapByExtension(false, false);
        }
    }

#if NETCOREAPP3_1_OR_GREATER
    public IEnumerable<IFileWrapperBase> EnumerateFiles(string searchPattern, EnumerationOptions searchOption)
    {
        foreach (var file in _directoryInfo.EnumerateFiles(searchPattern, searchOption))
        {
            yield return file.AutoWrapByExtension(false, false);
        }
    }
#endif

    public IEnumerable<IDirectoryWrapperBase> EnumerateDirectories()
    {
        foreach (var directory in _directoryInfo.EnumerateDirectories())
        {
            yield return new DirectoryWrapper(directory, false);
        }
    }

    public IEnumerable<IDirectoryWrapperBase> EnumerateDirectories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        foreach (var directory in _directoryInfo.EnumerateDirectories(searchPattern, searchOption))
        {
            yield return new DirectoryWrapper(directory, false);
        }
    }

#if NETCOREAPP3_1_OR_GREATER
    public IEnumerable<IDirectoryWrapperBase> EnumerateDirectories(string searchPattern, EnumerationOptions searchOption)
    {
        foreach (var directory in _directoryInfo.EnumerateDirectories(searchPattern, searchOption))
        {
            yield return new DirectoryWrapper(directory, false);
        }
    }
#endif

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
    {
        return _directoryInfo.EnumerateFileSystemInfos();
    }

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return _directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption);
    }

#if NETCOREAPP3_1_OR_GREATER
    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, EnumerationOptions searchOption)
    {
        return _directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption);
    }
#endif

    public IDirectoryWrapperBase[] GetDirectories()
    {
        return WrapDirectories(_directoryInfo.GetDirectories());
    }

    public IDirectoryWrapperBase[] GetDirectories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return WrapDirectories(_directoryInfo.GetDirectories(searchPattern, searchOption));
    }

#if NETCOREAPP3_1_OR_GREATER
    public IDirectoryWrapperBase[] GetDirectories(string searchPattern, EnumerationOptions searchOption)
    {
        return WrapDirectories(_directoryInfo.GetDirectories(searchPattern, searchOption));
    }
#endif

    public IFileWrapperBase[] GetFiles()
    {
        return WrapFiles(_directoryInfo.GetFiles());
    }

    public IFileWrapperBase[] GetFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return WrapFiles(_directoryInfo.GetFiles(searchPattern, searchOption));
    }

#if NETCOREAPP3_1_OR_GREATER
    public IFileWrapperBase[] GetFiles(string searchPattern, EnumerationOptions searchOption)
    {
        return WrapFiles(_directoryInfo.GetFiles(searchPattern, searchOption));
    }
#endif

    public FileSystemInfo[] GetFileSystemInfos()
    {
        return _directoryInfo.GetFileSystemInfos();
    }

    public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return _directoryInfo.GetFileSystemInfos(searchPattern, searchOption);
    }

#if NETCOREAPP3_1_OR_GREATER
    public FileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions searchOption)
    {
        return _directoryInfo.GetFileSystemInfos(searchPattern, searchOption);
    }
#endif

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

    private static IDirectoryWrapperBase[] WrapDirectories(DirectoryInfo[] directories)
    {
        var result = new IDirectoryWrapperBase[directories.Length];
        for (var i = 0; i < directories.Length; i++)
        {
            result[i] = new DirectoryWrapper(directories[i], false);
        }

        return result;
    }

    private static IFileWrapperBase[] WrapFiles(FileInfo[] files)
    {
        var result = new IFileWrapperBase[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            result[i] = files[i].AutoWrapByExtension(false, false);
        }

        return result;
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
        var fileWrapper = file.AutoWrapByExtension(false, false);
        CopyingFile.Publish(fileWrapper);
        await BufferedCopyAsync(fileWrapper, destination).ConfigureAwait(false);
        CopiedFile.Publish(fileWrapper);
    }
}
