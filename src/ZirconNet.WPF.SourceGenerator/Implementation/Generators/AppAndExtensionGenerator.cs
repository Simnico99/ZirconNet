using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZirconNet.WPF.SourceGenerator.Helpers;
using ZirconNet.WPF.SourceGenerator.Implementation.Models;

namespace ZirconNet.WPF.SourceGenerator.Implementation.Generators;
internal class AppAndExtensionGenerator : IZirconGenerator
{
    public void Generate(IncrementalGeneratorInitializationContext context)
    {
        var appClasses = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is ClassDeclarationSyntax { Identifier.ValueText: "App" } cds && cds.BaseList is not null,
            transform: static (ctx, _) =>
            {
                var classDeclaration = (ClassDeclarationSyntax)ctx.Node;
                if (ctx.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol symbol)
                {
                    return null;
                }
                for (var baseType = symbol.BaseType; baseType is not null; baseType = baseType.BaseType)
                {
                    if (baseType.ToDisplayString() == "System.Windows.Application")
                    {
                        return symbol;
                    }
                }
                return null;
            }).Where(symbol => symbol is not null);

        context.RegisterSourceOutput(appClasses.Collect(), (sourceContext, source) =>
        {
            var report = sourceContext.ReportDiagnostic;
            var appSymbol = source.FirstOrDefault();
            if (appSymbol is null)
            {
                return;
            }
            var appNamespace = appSymbol.ContainingNamespace.ToDisplayString();

            ZirconNetImplementationGenerator.Generate(
                new AppModel(appSymbol.Name, appNamespace, appSymbol.Name),
                sourceContext.AddSource,
                exception => DiagnosticHelpers.ReportDiagnostic(exception, report),
                "App");

            ZirconNetImplementationGenerator.Generate(
                new HostIntegrationModel(appNamespace, "WpfApplicationLifeTimeExtensions"),
                sourceContext.AddSource,
                exception => DiagnosticHelpers.ReportDiagnostic(exception, report),
                "WpfApplicationLifeTimeExtensions");
        });
    }
}
