using System.Diagnostics;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class JarFileWrapper : FileWrapperBase
{
    public JarFileWrapper(string file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }
    public JarFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false) : base(file, createFile, overwrite) { }

    public Process Run()
    {
        Process clientProcess = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $@"-jar {FullName} ",
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        clientProcess.Start();

        return clientProcess;
    }
}
