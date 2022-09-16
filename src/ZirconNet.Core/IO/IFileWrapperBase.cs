namespace ZirconNet.Core.IO;

public interface IFileWrapperBase
{
    bool Exists { get; }
    string FullName { get; }
    string Name { get; }

    StreamWriter AppendText();
    Task CopyToDirectoryAsync(IDirectoryWrapperBase directory);
    StreamWriter CreateText();
    void Delete();
    byte[] GetByteArray();
    FileStream Open(FileMode fileMode, FileAccess access = 0, FileShare share = FileShare.None);
    FileStream OpenRead();
    StreamReader OpenText();
    FileStream OpenWrite();
}