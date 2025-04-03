using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ZirconNet.WPF.SourceGenerator.Helpers;

internal readonly struct CompilationAnalyzerContext(Compilation Compilation, IReadOnlyList<InvocationExpressionSyntax>? AddMediatorCalls, string GeneratorVersion, Action<Diagnostic> ReportDiagnostic, CancellationToken CancellationToken)
{
    public Compilation Compilation { get; } = Compilation;
    public IReadOnlyList<InvocationExpressionSyntax>? AddMediatorCalls { get; } = AddMediatorCalls;
    public string GeneratorVersion { get; } = GeneratorVersion;
    public Action<Diagnostic> ReportDiagnostic { get; } = ReportDiagnostic;
    public CancellationToken CancellationToken { get; } = CancellationToken;
}

internal static class DiagnosticHelpers
{
    private static long _counter;
    private static readonly HashSet<string> _ids = default!;

    public static IReadOnlyCollection<string> Ids => _ids;

    private static string GetNextId()
    {
        var count = _counter++;
        var id = $"Z{count.ToString().PadLeft(4, '0')}";
        _ids?.Add(id);

        return id;
    }

    static DiagnosticHelpers()
    {
        GenericError = new(
            GetNextId(),
            $"{nameof(SourceGenerator)} unknown error",
            $"{nameof(SourceGenerator)} got unknown error while generating implementation, Error: " + "{0}",
            nameof(SourceGenerator),
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }

    public static readonly DiagnosticDescriptor GenericError;

    internal static void ReportDiagnostic(Exception exception, Action<Diagnostic>? report) => report?.ReportGenericError(exception);

    internal static Diagnostic ReportGenericError(this CompilationAnalyzerContext context, Exception exception)
    {
        var error =
            $"{exception.Message}: {exception.StackTrace}{(exception.InnerException is not null ? $"\nInner: {exception.InnerException}" : "")}";
        var diagnostic = Diagnostic.Create(GenericError, Location.None, error);
        context.ReportDiagnostic(diagnostic);
        return diagnostic;
    }

    internal static void ReportGenericError(this Action<Diagnostic> reportError, Exception exception)
    {
        var error =
            $"{exception.Message}: {exception.StackTrace}{(exception.InnerException is not null ? $"\nInner: {exception.InnerException}" : "")}";
        var diagnostic = Diagnostic.Create(GenericError, Location.None, error);
        reportError(diagnostic);
    }
}
