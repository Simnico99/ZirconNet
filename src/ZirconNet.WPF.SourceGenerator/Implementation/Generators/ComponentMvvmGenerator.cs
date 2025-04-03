using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZirconNet.WPF.SourceGenerator.Helpers;
using ZirconNet.WPF.SourceGenerator.Implementation.Models;

namespace ZirconNet.WPF.SourceGenerator.Implementation.Generators;

internal class ComponentMvvmGenerator : IZirconGenerator
{
    public void Generate(IncrementalGeneratorInitializationContext context)
    {
        var componentCandidates = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => node is ClassDeclarationSyntax,
            transform: (ctx, _) =>
            {
                var classSyntax = (ClassDeclarationSyntax)ctx.Node;
                if (ctx.SemanticModel.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol symbol)
                {
                    return null;
                }
                var attr = symbol.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.Name == "PageDataContextAttribute");
                return attr is not null ? new ComponentBindingInfo(symbol, attr) : null;
            }).Where(info => info is not null)!;

        context.RegisterSourceOutput(componentCandidates.Collect(), (sourceContext, source) =>
        {
            var report = sourceContext.ReportDiagnostic;
            var pages = source.SelectMany(binding => binding!.Pages.Select(page => (Page: page!, binding.ViewModel)))
                              .GroupBy(x => x.Page, SymbolEqualityComparer.Default);

            foreach (var group in pages)
            {
                var componentType = (INamedTypeSymbol?)group.Key;
                var viewModels = group.Select(x => x.ViewModel);

                if (componentType is null)
                {
                    return;
                }

                var propertyDeclarations = new StringBuilder();
                foreach (var viewModel in viewModels)
                {
                    var ns = viewModel.ContainingNamespace.ToDisplayString();
                    var vmType = viewModel.Name;
                    propertyDeclarations.AppendLine($"    public {ns}.{vmType} {vmType} => _appHost.Services.GetRequiredService<{ns}.{vmType}>();");
                }

                ZirconNetImplementationGenerator.Generate(
                    new ComponentMvvmModel(componentType.Name, componentType.ContainingNamespace.ToDisplayString(), propertyDeclarations.ToString(), componentType.Name),
                    sourceContext.AddSource,
                    exception => DiagnosticHelpers.ReportDiagnostic(exception, report),
                    "ComponentMvvm");

                propertyDeclarations.Clear();
            }
        });
    }

    /// <summary>
    /// Represents binding information for a page and its associated view model.
    /// </summary>
    private sealed class ComponentBindingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBindingInfo"/> class.
        /// </summary>
        /// <param name="viewModel">The view model symbol.</param>
        /// <param name="attr">The attribute data associated with the view model.</param>
        public ComponentBindingInfo(INamedTypeSymbol viewModel, AttributeData attr)
        {
            ViewModel = viewModel;
            Pages = attr.ConstructorArguments.FirstOrDefault().Values
                       .Select(tc => tc.Value as INamedTypeSymbol)
                       .Where(t => t is not null)
                       .ToImmutableArray()!;
        }

        /// <summary>
        /// Gets the view model symbol.
        /// </summary>
        public INamedTypeSymbol ViewModel { get; }

        /// <summary>
        /// Gets the collection of page symbols bound to the view model.
        /// </summary>
        public ImmutableArray<INamedTypeSymbol> Pages { get; }
    }
}
