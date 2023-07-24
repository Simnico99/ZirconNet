// <copyright file="JarFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Diagnostics;
using System.Runtime.Versioning;

namespace ZirconNet.Core.IO;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public sealed class JarFileWrapper : FileWrapperBase
{
    public JarFileWrapper(string file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    public JarFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    public Process Run()
    {
        Process clientProcess = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $@"-jar {FullName} ",
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        _ = clientProcess.Start();

        return clientProcess;
    }
}
