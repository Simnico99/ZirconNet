using Microsoft.CodeAnalysis;

namespace ZirconNet.WPF.SourceGenerator.Implementation.Generators;
internal interface IZirconGenerator
{
    void Generate(IncrementalGeneratorInitializationContext context);
}
