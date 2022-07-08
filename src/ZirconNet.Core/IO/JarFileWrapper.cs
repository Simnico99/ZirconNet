using System.Diagnostics;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public class JarFileWrapper : FileWrapper
{
    public JarFileWrapper(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public JarFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }

    public Process Run()
    {
        Process clientProcess = new();

        clientProcess.StartInfo = new ProcessStartInfo
        {
            FileName = "java",
            Arguments = $@"-jar {FullName} ",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        clientProcess.Start();

        return clientProcess;
    }
}
