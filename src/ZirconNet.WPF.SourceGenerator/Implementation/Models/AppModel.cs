namespace ZirconNet.WPF.SourceGenerator.Implementation.Models;
internal sealed class AppModel(string ClassName, string NamespaceName, string GeneratedFileName) : ICompilationModel
{
    public string ClassName { get; } = ClassName;
    public string NamespaceName { get; } = NamespaceName;
    public string GeneratedFileName { get; } = GeneratedFileName;
}
