using System.Security.AccessControl;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO
{
    public interface IDirectoryWrapperBase
    {
        IWeakEvent<IFileWrapperBase> CopiedFile { get; }
        IWeakEvent<IFileWrapperBase> CopyingFile { get; }
        bool Exists { get; }
        string FullName { get; }
        string Name { get; }

        Task CopyContentAsync(IDirectoryWrapperBase destination);
        void Delete();
        IEnumerable<IDirectoryWrapperBase> EnumerateDirectories();
        IEnumerable<IFileWrapperBase> EnumerateFiles();
        IEnumerable<FileSystemInfo> EnumerateFileSystemInfos();
        DirectorySecurity GetAccessControl();
        IDirectoryWrapperBase[] GetDirectories();
        IFileWrapperBase[] GetFiles();
        void SetAccessControl(DirectorySecurity directorySecurity);
    }
}