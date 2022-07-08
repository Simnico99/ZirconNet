using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.IO;
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
