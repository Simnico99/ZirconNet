// <copyright file="IDirectoryWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Security.AccessControl;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;

/// <summary>
/// Represents a directory wrapper with generic functionality.
/// </summary>
/// <typeparam name="T">The specific type of directory wrapper.</typeparam>
public interface IDirectoryWrapperBase<T> : IDirectoryWrapperBase
    where T : DirectoryWrapperBase<T>
{
    /// <summary>
    /// Gets the parent directory.
    /// </summary>
    /// <returns>The parent directory, or null if there is no parent.</returns>
    T? Parent { get; }

    /// <summary>
    /// Gets the root directory.
    /// </summary>
    /// <returns>The root directory.</returns>
    T Root { get; }
}

/// <summary>
/// Represents a directory wrapper with generic functionality.
/// </summary>
public interface IDirectoryWrapperBase
{
    /// <summary>
    /// Gets the event that is raised when a file is copied.
    /// </summary>
    IWeakEvent<IFileWrapperBase> CopiedFile { get; }

    /// <summary>
    /// Gets the event that is raised when a file is about to be copied.
    /// </summary>
    IWeakEvent<IFileWrapperBase> CopyingFile { get; }

    /// <summary>
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets the full name of the directory.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the name of the directory.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Copies the content of the directory asynchronously.
    /// </summary>
    /// <param name="destination">The destination directory.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask CopyContentAsync(IDirectoryWrapperBase destination);

    /// <summary>
    /// Deletes the directory.
    /// </summary>
    void Delete();

    /// <summary>
    /// Deletes the directory, optionally deleting its contents recursively.
    /// </summary>
    /// <param name="recursive">Determines whether to delete the contents recursively.</param>
    void Delete(bool recursive);

    /// <summary>
    /// Enumerates the directories in the current directory.
    /// </summary>
    /// <returns>An enumerable collection of directory wrappers.</returns>
    IEnumerable<IDirectoryWrapperBase> EnumerateDirectories();

    /// <summary>
    /// Enumerates the directories in the current directory that match the specified search pattern and search option.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The search option to use (optional).</param>
    /// <returns>An enumerable collection of directory wrappers.</returns>
    IEnumerable<IDirectoryWrapperBase> EnumerateDirectories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Enumerates the directories in the current directory that match the specified search pattern and enumeration options (for .NET Core 3.1 or greater).
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The enumeration options to use.</param>
    /// <returns>An enumerable collection of directory wrappers.</returns>
    IEnumerable<IDirectoryWrapperBase> EnumerateDirectories(string searchPattern, EnumerationOptions searchOption);
#endif

    /// <summary>
    /// Enumerates the files in the current directory.
    /// </summary>
    /// <returns>An enumerable collection of file wrappers.</returns>
    IEnumerable<IFileWrapperBase> EnumerateFiles();

    /// <summary>
    /// Enumerates the files in the current directory that match the specified search pattern and search option.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The search option to use (optional).</param>
    /// <returns>An enumerable collection of file wrappers.</returns>
    IEnumerable<IFileWrapperBase> EnumerateFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Enumerates the files in the current directory that match the specified search pattern and enumeration options (for .NET Core 3.1 or greater).
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The enumeration options to use.</param>
    /// <returns>An enumerable collection of file wrappers.</returns>
    IEnumerable<IFileWrapperBase> EnumerateFiles(string searchPattern, EnumerationOptions searchOption);
#endif

    // Similar comments apply to the rest of the methods
    // ...

    /// <summary>
    /// Gets the access control for the directory.
    /// </summary>
    /// <returns>The directory's access control.</returns>
    DirectorySecurity GetAccessControl();

    /// <summary>
    /// Gets the directories in the current directory.
    /// </summary>
    /// <returns>An array of directory wrappers.</returns>
    IDirectoryWrapperBase[] GetDirectories();

    /// <summary>
    /// Gets the directories in the current directory that match the specified search pattern and search option.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The search option to use (optional).</param>
    /// <returns>An array of directory wrappers.</returns>
    IDirectoryWrapperBase[] GetDirectories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Gets the directories in the current directory that match the specified search pattern and enumeration options (for .NET Core 3.1 or greater).
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The enumeration options to use.</param>
    /// <returns>An array of directory wrappers.</returns>
    IDirectoryWrapperBase[] GetDirectories(string searchPattern, EnumerationOptions searchOption);
#endif

    /// <summary>
    /// Gets the files in the current directory.
    /// </summary>
    /// <returns>An array of file wrappers.</returns>
    IFileWrapperBase[] GetFiles();

    /// <summary>
    /// Gets the files in the current directory that match the specified search pattern and search option.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The search option to use (optional).</param>
    /// <returns>An array of file wrappers.</returns>
    IFileWrapperBase[] GetFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Gets the files in the current directory that match the specified search pattern and enumeration options (for .NET Core 3.1 or greater).
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The enumeration options to use.</param>
    /// <returns>An array of file wrappers.</returns>
    IFileWrapperBase[] GetFiles(string searchPattern, EnumerationOptions searchOption);
#endif

    /// <summary>
    /// Gets the file system information objects in the current directory.
    /// </summary>
    /// <returns>An array of FileSystemInfo objects.</returns>
    FileSystemInfo[] GetFileSystemInfos();

    /// <summary>
    /// Gets the file system information objects in the current directory that match the specified search pattern and search option.
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The search option to use (optional).</param>
    /// <returns>An array of FileSystemInfo objects.</returns>
    FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Gets the file system information objects in the current directory that match the specified search pattern and enumeration options (for .NET Core 3.1 or greater).
    /// </summary>
    /// <param name="searchPattern">The search pattern to use.</param>
    /// <param name="searchOption">The enumeration options to use.</param>
    /// <returns>An array of FileSystemInfo objects.</returns>
    FileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions searchOption);
#endif

    /// <summary>
    /// Moves the directory to a new location.
    /// </summary>
    /// <param name="destDirName">The destination directory name.</param>
    void MoveTo(string destDirName);

    /// <summary>
    /// Sets the access control for the directory.
    /// </summary>
    /// <param name="directorySecurity">The directory security to apply.</param>
    void SetAccessControl(DirectorySecurity directorySecurity);
}