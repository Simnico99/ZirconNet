namespace ZirconNet.WPF.SourceGenerator.Implementation.Models;
internal sealed class HostIntegrationModel(string AppNamespace, string GeneratedFileName) : ICompilationModel
{
    public string AppNamespace { get; } = AppNamespace;
    public string GeneratedFileName { get; } = GeneratedFileName;
}
