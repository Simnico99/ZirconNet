namespace ZirconNet.WPF.SourceGenerator.Implementation.Models;
internal sealed class ComponentMvvmModel(string ComponentName, string ComponentNamespace, string PropertyPropertyDeclarations, string GeneratedFileName) : ICompilationModel
{
    public string ComponentName { get; } = ComponentName;
    public string ComponentNamespace { get; } = ComponentNamespace;
    public string PropertyDeclarations { get; } = PropertyPropertyDeclarations;
    public string GeneratedFileName { get; } = GeneratedFileName;
}
