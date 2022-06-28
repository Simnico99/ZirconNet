using System.Security.AccessControl;
using System.Security.Principal;
using ZirconNet.Core.Events;

namespace ZirconNet.Core.IO;
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

    private static void SetAccess(DirectoryInfo directory)
    {
        var fSecurity = directory.GetAccessControl();
        fSecurity.AddAccessRule(new FileSystemAccessRule("Utilisateurs", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
        fSecurity.AddAccessRule(new FileSystemAccessRule("Utilisateurs", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
        fSecurity.AddAccessRule(new FileSystemAccessRule("Utilisateurs", FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.InheritOnly, AccessControlType.Allow));
        directory.SetAccessControl(fSecurity);
    }

    public bool IsDirectoryWritable()
    {
        try
        {
            var acl = _directoryInfo.GetAccessControl();
            var rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            var currentUser = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(currentUser);
            foreach (AuthorizationRule rule in rules)
            {
                if (rule is not FileSystemAccessRule fsAccessRule)
                {
                    continue;
                }

                if (fsAccessRule.FileSystemRights == FileSystemRights.FullControl && rule.IdentityReference is not null)
                {
                    var ntAccount = (NTAccount)rule.IdentityReference;
                    if (ntAccount == null)
                    {
                        continue;
                    }

                    if (principal.IsInRole(ntAccount.Value))
                    {
                        if (ntAccount.Value == @"BUILTIN\Utilisateurs")
                        {
                            return true;
                        }
                        continue;
                    }
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
            new DirectoryInfo(path).Delete();
        }

        if (Directory.Exists(path) && !overwrite)
        {
            return new DirectoryInfo(path);
        }

        Directory.CreateDirectory(path);
        DirectoryInfo directory = new(path);
        SetAccess(directory);

        return directory;
    }

    public async Task CopyContentAsync(DirectoryWrapper destination)
    {
        if (_directoryInfo is not null)
        {
            foreach (var file in _directoryInfo.GetFiles())
            {
                var fileWrapper = new FileWrapper(file);
                CopyingFile.Publish(fileWrapper);
                await fileWrapper.CopyToDirectoryAsync(destination);
                CopiedFile.Publish(fileWrapper);
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
}
