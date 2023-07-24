// <copyright file="IFileWrapperBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.IO;

/// <summary>
/// Base interface for file wrappers.
/// </summary>
public interface IFileWrapperBase
{
    /// <summary>
    /// Gets a value indicating whether the file exists.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets the full name of the file including its path.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Appends text to the file, and returns a StreamWriter that writes to the file.
    /// </summary>
    /// <returns>A StreamWriter that writes to the file.</returns>
    StreamWriter AppendText();

    /// <summary>
    /// Asynchronously copies the file to a specified directory.
    /// </summary>
    /// <param name="directory">The directory to which the file will be copied.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    ValueTask CopyToDirectoryAsync(IDirectoryWrapperBase directory);

    /// <summary>
    /// Creates a new file, writes a specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <returns>A StreamWriter that writes to the new file.</returns>
    StreamWriter CreateText();

    /// <summary>
    /// Deletes the file.
    /// </summary>
    void Delete();

    /// <summary>
    /// Returns the content of the file as a byte array.
    /// </summary>
    /// <returns>A byte array containing the contents of the file.</returns>
    byte[] GetByteArray();

    /// <summary>
    /// Opens the file with the specified file mode, access and sharing permission.
    /// </summary>
    /// <param name="fileMode">A FileMode value that specifies the mode in which to open a file.</param>
    /// <param name="access">A FileAccess value that specifies the operations that can be performed on the file.</param>
    /// <param name="share">A FileShare value that specifies the type of access other threads have to the file.</param>
    /// <returns>A FileStream on the specified path, having the specified mode with read, write, or read/write access and the specified sharing option.</returns>
    FileStream Open(FileMode fileMode, FileAccess access = 0, FileShare share = FileShare.None);

    /// <summary>
    /// Opens an existing file for reading.
    /// </summary>
    /// <returns>A read-only FileStream on the specified path.</returns>
    FileStream OpenRead();

    /// <summary>
    /// Opens an existing text file for reading.
    /// </summary>
    /// <returns>A StreamReader on the specified path.</returns>
    StreamReader OpenText();

    /// <summary>
    /// Opens an existing file or creates a new file for writing.
    /// </summary>
    /// <returns>An unshared FileStream object on the specified path with Write access.</returns>
    FileStream OpenWrite();
}
