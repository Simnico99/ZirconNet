#nullable enable
using Microsoft.CodeAnalysis;
using ZirconNet.WPF.SourceGenerator.Implementation.Generators;

namespace ZirconNet.WPF.SourceGenerator;

[Generator]
public sealed class IncrementalZirconNetWPFGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}

        CheckForRequiredDependencies(context);

        var assembly = typeof(IZirconGenerator).Assembly;
        var generatorTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IZirconGenerator).IsAssignableFrom(t));

        var generators = new List<IZirconGenerator>();

        foreach (var type in generatorTypes)
        {
            var constructor = type.GetConstructor([]) ?? throw new InvalidOperationException($"Type {type.FullName} does not have a public parameterless constructor.");
            var generator = (IZirconGenerator)Activator.CreateInstance(type)!;
            generators.Add(generator);
            generator.Generate(context);
        }
    }

    private void CheckForRequiredDependencies(IncrementalGeneratorInitializationContext context)
    {
        var isWpfReferencedProvider = context.CompilationProvider.Select((compilation, ct) => compilation.ReferencedAssemblyNames
        .Any(a => a.Name.Equals("ZirconNet.WPF", StringComparison.OrdinalIgnoreCase)));

        context.RegisterImplementationSourceOutput(isWpfReferencedProvider, (spc, isWpfReferenced) =>
        {
            if (!isWpfReferenced)
            {
                var descriptor = new DiagnosticDescriptor(
                    id: "Z0001",
                    title: "Missing Dependency",
                    messageFormat: "ZirconNet.WPF is required but not present. Please add a reference to ZirconNet.WPF.",
                    category: "ZirconGenerator",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true);
                spc.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
            }
        });
    }
}
