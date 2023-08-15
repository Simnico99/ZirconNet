// <copyright file="JarFileWrapper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Diagnostics;

namespace ZirconNet.Core.IO;

public sealed class JarFileWrapper : FileWrapperBase<JarFileWrapper>
{
    public JarFileWrapper(string file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    public JarFileWrapper(FileInfo file, bool createFile = true, bool overwrite = false)
        : base(file, createFile, overwrite)
    {
    }

    /// <summary>
    /// Run the jar file.
    /// </summary>
    /// <returns>The current running process.</returns>
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
