namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class FileWrapper : FileWrapperBase
{
    public FileWrapper(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public FileWrapper(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
}

