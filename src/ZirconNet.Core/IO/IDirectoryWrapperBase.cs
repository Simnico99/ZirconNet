// <copyright file="IDirectoryWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Security.AccessControl;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;

/// <summary>
/// Directory wrapper base interface.
/// </summary>
public interface IDirectoryWrapperBase
{
    /// <summary>
    /// Gets an event that occurs when a file has been copied.
    /// </summary>
    IWeakEvent<IFileWrapperBase> CopiedFile { get; }

    /// <summary>
    /// Gets an event that occurs when a file is being copied.
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
    /// Asynchronously copies the content of the directory to a destination.
    /// </summary>
    /// <param name="destination">The destination directory.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    ValueTask CopyContentAsync(IDirectoryWrapperBase destination);

    /// <summary>
    /// Deletes the directory.
    /// </summary>
    void Delete();

    /// <summary>
    /// Enumerates all the subdirectories within the directory.
    /// </summary>
    /// <returns>A sequence of directories in the directory.</returns>
    IEnumerable<IDirectoryWrapperBase> EnumerateDirectories();

    /// <summary>
    /// Enumerates all the files within the directory.
    /// </summary>
    /// <returns>A sequence of files in the directory.</returns>
    IEnumerable<IFileWrapperBase> EnumerateFiles();

    /// <summary>
    /// Enumerates all the file system information within the directory.
    /// </summary>
    /// <returns>A sequence of file system information in the directory.</returns>
    IEnumerable<FileSystemInfo> EnumerateFileSystemInfos();

    /// <summary>
    /// Gets the access control of the directory.
    /// </summary>
    /// <returns>A DirectorySecurity object that encapsulates the access control rules for the directory.</returns>
    DirectorySecurity GetAccessControl();

    /// <summary>
    /// Gets all the subdirectories within the directory.
    /// </summary>
    /// <returns>An array of directories in the directory.</returns>
    IDirectoryWrapperBase[] GetDirectories();

    /// <summary>
    /// Gets all the files within the directory.
    /// </summary>
    /// <returns>An array of files in the directory.</returns>
    IFileWrapperBase[] GetFiles();

    /// <summary>
    /// Sets the access control for the directory.
    /// </summary>
    /// <param name="directorySecurity">A DirectorySecurity object that contains the access control rules to apply to the directory.</param>
    void SetAccessControl(DirectorySecurity directorySecurity);
}
