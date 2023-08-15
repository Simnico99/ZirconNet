// <copyright file="IFileWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
#endif

namespace ZirconNet.Core.IO;

/// <summary>
/// Represents a file wrapper with generic functionality.
/// </summary>
/// <typeparam name="T">The specific type of file wrapper.</typeparam>
public interface IFileWrapperBase<T> : IFileWrapperBase
    where T : FileWrapperBase<T>
{
    /// <summary>
    /// Copies the current file to the specified path, optionally overwriting the existing file.
    /// </summary>
    /// <param name="outputFilePath">The destination path for the copy.</param>
    /// <param name="overwrite">Determines whether to overwrite the file if it already exists.</param>
    /// <returns>A new file wrapper representing the copied file.</returns>
    T CopyTo(string outputFilePath, bool overwrite = false);

    /// <summary>
    /// Replaces the current file with another file, optionally backing up the original file and ignoring metadata errors.
    /// </summary>
    /// <param name="destinationFileName">The name of the destination file.</param>
    /// <param name="destinationBackupFileName">The name of the backup file, if any.</param>
    /// <param name="ignoreMetadataErrors">Determines whether to ignore metadata errors.</param>
    /// <returns>A new file wrapper representing the replaced file.</returns>
    T Replace(string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors = false);
}

/// <summary>
/// Represents the base functionality for a file wrapper.
/// </summary>
public interface IFileWrapperBase
{
    /// <summary>Gets the directory information.</summary>
    IDirectoryWrapperBase? Directory { get; }

    /// <summary>Gets the directory name.</summary>
    string? DirectoryName { get; }

    /// <summary>Gets a value indicating whether the file exists.</summary>
    bool Exists { get; }

    /// <summary>Gets the full name of the file.</summary>
    string FullName { get; }

    /// <summary>Gets or sets a value indicating whether the file is read-only.</summary>
    bool IsReadOnly { get; set; }

    /// <summary>Gets the file length in bytes.</summary>
    long Length { get; }

    /// <summary>Gets the name of the file.</summary>
    string Name { get; }

    /// <summary>Appends text to the file.</summary>
    /// <returns>A StreamWriter object to write to the file.</returns>
    StreamWriter AppendText();

    /// <summary>
    /// Copies the file to a specified directory asynchronously.
    /// </summary>
    /// <param name="directory">The destination directory.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask CopyToDirectoryAsync(IDirectoryWrapperBase directory);

    /// <summary>Creates a StreamWriter for writing text to the file.</summary>
    /// <returns>A StreamWriter object to write to the file.</returns>
    StreamWriter CreateText();

    /// <summary>Deletes the file.</summary>
    void Delete();

    /// <summary>
    /// Read the file contents as a byte array.
    /// </summary>
    /// <returns>The file contents as a byte array.</returns>
    byte[] ReadAllBytes();

#if NETCOREAPP3_1_OR_GREATER
    /// <summary>
    /// Moves the file to a new location.
    /// </summary>
    /// <param name="outputFilePath">The destination path for the move.</param>
    /// <param name="overwrite">Determines whether to overwrite the file if it already exists at the destination.</param>
    void MoveTo(string outputFilePath, bool overwrite = false);
#else
    /// <summary>
    /// Moves the file to a new location.
    /// </summary>
    /// <param name="outputFilePath">The destination path for the move.</param>
    void MoveTo(string outputFilePath);
#endif

    /// <summary>
    /// Opens the file with the specified mode and access.
    /// </summary>
    /// <param name="fileMode">The file mode to use.</param>
    /// <param name="access">The file access to use.</param>
    /// <param name="share">The file share to use.</param>
    /// <returns>A FileStream object associated with the file.</returns>
    FileStream Open(FileMode fileMode, FileAccess access = 0, FileShare share = FileShare.None);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Opens the file with the specified options.
    /// </summary>
    /// <param name="fileStreamOptions">The file stream options to use.</param>
    /// <returns>A FileStream object associated with the file.</returns>
    FileStream Open(FileStreamOptions fileStreamOptions);
#endif

    /// <summary>
    /// Opens the file for reading.
    /// </summary>
    /// <returns>A FileStream object for reading from the file.</returns>
    FileStream OpenRead();

    /// <summary>
    /// Opens the file for reading text.
    /// </summary>
    /// <returns>A StreamReader object for reading text from the file.</returns>
    StreamReader OpenText();

    /// <summary>
    /// Opens the file for writing.
    /// </summary>
    /// <returns>A FileStream object for writing to the file.</returns>
    FileStream OpenWrite();

#if NET5_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance of the <see cref="Microsoft.Win32.SafeHandles.SafeFileHandle" /> class with the specified path, creation mode, read/write and sharing permission, the access other SafeFileHandles can have to the same file, additional file options and the allocation size.
    /// </summary>
    /// <param name="mode">One of the enumeration values that determines how to open or create the file. The default value is <see cref="FileMode.Open" />.</param>
    /// <param name="access">A bitwise combination of the enumeration values that determines how the file can be accessed. The default value is <see cref="FileAccess.Read" />.</param>
    /// <param name="share">A bitwise combination of the enumeration values that determines how the file will be shared by processes. The default value is <see cref="FileShare.Read" />.</param>
    /// <param name="options">An object that describes optional <see cref="Microsoft.Win32.SafeHandles.SafeFileHandle" /> parameters to use.</param>
    /// <param name="preallocationSize">The initial allocation size in bytes for the file. A positive value is effective only when a regular file is being created, overwritten, or replaced.
    /// Negative values are not allowed. In other cases (including the default 0 value), it's ignored.</param>
    /// <returns>A <see cref="SafeFileHandle"/>.</returns>
    public SafeFileHandle OpenHandle(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, FileOptions options = FileOptions.None, long preallocationSize = 0);
#endif

    /// <summary>
    /// Sets the date and time the file or directory was created.
    /// </summary>
    /// <param name="creationTime">
    /// A <see cref="DateTime"/> containing the value to set for the creation date and time of <paramref name="fileHandle"/>.
    /// This value is expressed in local time.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="creationTime"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetCreationTime(DateTime creationTime);

    /// <summary>
    /// Sets the date and time, in coordinated universal time (UTC), that the file or directory was created.
    /// </summary>
    /// <param name="creationTimeUtc">
    /// A <see cref="DateTime"/> containing the value to set for the creation date and time.
    /// This value is expressed in UTC time.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="creationTimeUtc"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetCreationTimeUtc(DateTime creationTimeUtc);

    /// <summary>
    /// Returns the creation date and time of the specified file or directory.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the creation date and time for the specified file or
    /// directory. This value is expressed in local time.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileHandle"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    public DateTime GetCreationTime();

    /// <summary>
    /// Returns the creation date and time, in coordinated universal time (UTC), of the specified file or directory.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the creation date and time for the specified file or
    /// directory. This value is expressed in UTC time.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileHandle"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    public DateTime GetCreationTimeUtc();

    /// <summary>
    /// Sets the date and time the specified file or directory was last accessed.
    /// </summary>
    /// <param name="lastAccessTime">
    /// A <see cref="DateTime"/> containing the value to set for the last access date and time.
    /// This value is expressed in local time.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileHandle"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lastAccessTime"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetLastAccessTime(DateTime lastAccessTime);

    /// <summary>
    /// Sets the date and time, in coordinated universal time (UTC), that the specified file or directory was last accessed.
    /// </summary>
    /// <param name="lastAccessTimeUtc">
    /// A <see cref="DateTime"/> containing the value to set for the last access date and time.
    /// This value is expressed in UTC time.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileHandle"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lastAccessTimeUtc"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetLastAccessTimeUtc(DateTime lastAccessTimeUtc);

    /// <summary>
    /// Returns the last access date and time of the specified file or directory.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the last access date and time for the specified file or
    /// directory. This value is expressed in local time.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    DateTime GetLastAccessTime();

    /// <summary>
    /// Returns the last access date and time, in coordinated universal time (UTC), of the specified file or directory.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the last access date and time for the specified file or
    /// directory. This value is expressed in UTC time.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    DateTime GetLastAccessTimeUtc();

    /// <summary>
    /// Sets the date and time that the specified file or directory was last written to.
    /// </summary>
    /// <param name="lastWriteTime">
    /// A <see cref="DateTime"/> containing the value to set for the last write date and time.
    /// This value is expressed in local time.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lastWriteTime"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetLastWriteTime(DateTime lastWriteTime);

    /// <summary>
    /// Sets the date and time, in coordinated universal time (UTC), that the specified file or directory was last written to.
    /// </summary>
    /// <param name="lastWriteTimeUtc">
    /// A <see cref="DateTime"/> containing the value to set for the last write date and time.
    /// This value is expressed in UTC time.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lastWriteTimeUtc"/> specifies a value outside the range of dates, times, or both permitted for this operation.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while performing the operation.
    /// </exception>
    void SetLastWriteTimeUtc(DateTime lastWriteTimeUtc);

    /// <summary>
    /// Returns the last write date and time of the specified file.
    /// </summary
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the last write date and time for the specified file. This value is expressed in local time.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    DateTime GetLastWriteTime();

    /// <summary>
    /// Returns the last write date and time, in coordinated universal time (UTC), of the specified file.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTime" /> structure set to the last write date and time for the specified file. This value is expressed in UTC time.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    DateTime GetLastWriteTimeUtc();

    /// <summary>
    /// Gets the specified <see cref="FileAttributes"/> of the file.
    /// </summary>
    /// <returns>
    /// The <see cref="FileAttributes"/> of the file.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileHandle"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    FileAttributes GetAttributes();

    /// <summary>
    /// Sets the specified <see cref="FileAttributes"/> of the file.
    /// </summary>
    /// <param name="fileAttributes">
    /// A bitwise combination of the enumeration values.
    /// </param>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    void SetAttributes(FileAttributes fileAttributes);

    /// <summary>
    /// Decrypt the file on Windows.
    /// </summary>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    void Decrypt();

    /// <summary>
    /// Encrypt the file on Windows.
    /// </summary>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    void Encrypt();
}
