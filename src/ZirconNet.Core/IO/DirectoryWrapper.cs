using System.Security.AccessControl;
using System.Security.Principal;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;
#if NET5_0_OR_GREATER
[SupportedOSPlatform("Windows")]
#endif
public class DirectoryWrapper : FileSystemInfo
{
    protected DirectoryInfo _directoryInfo;
    public override string FullName => _directoryInfo.FullName;
    public override string Name => _directoryInfo.Name;
    public override bool Exists => _directoryInfo.Exists;
    public IWeakEvent<FileWrapper> CopyingFile { get; } = new WeakEvent<FileWrapper>();
    public IWeakEvent<FileWrapper> CopiedFile { get; } = new WeakEvent<FileWrapper>();

    public DirectoryWrapper(string directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory, overwrite) : new DirectoryInfo(directory);
    }

    public DirectoryWrapper(DirectoryInfo directory, bool createDirectory = true, bool overwrite = false)
    {
        _directoryInfo = createDirectory ? Create(directory.FullName, overwrite) : directory;
    }

    public bool IsDirectoryWritable()
    {
        try
        {
            var acl = _directoryInfo.GetAccessControl();
            var rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            var currentUser = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(currentUser);
            foreach (AuthorizationRule? rule in rules)
            {
                if (rule is not FileSystemAccessRule fsAccessRule || fsAccessRule.FileSystemRights != FileSystemRights.FullControl || rule.IdentityReference is null)
                {
                    continue;
                }

                var ntAccount = (NTAccount)rule.IdentityReference;

                if (ntAccount == null || !principal.IsInRole(ntAccount.Value))
                {
                    continue;
                }

                if (ntAccount.Value == @"BUILTIN\Utilisateurs")
                {
                    return true;
                }
            }
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static DirectoryInfo Create(string path, bool overwrite = false)
    {
        if (Directory.Exists(path) && overwrite)
        {
            new DirectoryInfo(path).Delete(true);
        }

        if (Directory.Exists(path) && !overwrite)
        {
            return new DirectoryInfo(path);
        }

        Directory.CreateDirectory(path);
        DirectoryInfo directory = new(path);

        return directory;
    }

    public async Task CopyContentAsync(DirectoryWrapper destination)
    {
        if (_directoryInfo is not null)
        {
            foreach (var file in _directoryInfo.GetFiles())
            {
                var fileWrapper = new FileWrapper(file);
                await CopyingFile.PublishAsync(fileWrapper);
                await fileWrapper.CopyToDirectoryAsync(destination);
                await CopiedFile.PublishAsync(fileWrapper);
            }

            foreach (var folder in _directoryInfo.GetDirectories())
            {
                var folderModel = new DirectoryWrapper(folder, false);
                await folderModel.CopyContentAsync(destination);
            }
        }
    }

    public override void Delete()
    {
        if (_directoryInfo is not null && Exists)
        {
            _directoryInfo.Delete(true);
        }
    }

    public IEnumerable<FileWrapper> EnumerateFiles() 
    {
        foreach (var file in _directoryInfo.EnumerateFiles())
        { 
            yield return new FileWrapper(file, false);
        }
    }

    public IEnumerable<DirectoryWrapper> EnumerateDirectories()
    {
        foreach (var directory in _directoryInfo.EnumerateDirectories())
        {
            yield return new DirectoryWrapper(directory, false);
        }
    }

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() 
    {
        return _directoryInfo.EnumerateFileSystemInfos();
    }

    public DirectorySecurity GetAccessControl() 
    {
        return _directoryInfo.GetAccessControl();
    }

    public void SetAccessControl(DirectorySecurity directorySecurity)
    {
        _directoryInfo.SetAccessControl(directorySecurity);
    }
}
