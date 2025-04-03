using System.Text;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;
using ZirconNet.WPF.SourceGenerator.Implementation.Models;

namespace ZirconNet.WPF.SourceGenerator.Implementation;

/// <summary>
/// Provides functionality for generating source code using Scriban templates.
/// </summary>
internal static class ZirconNetImplementationGenerator
{
    /// <summary>
    /// Generates source code from a Scriban template based on the provided compilation model.
    /// </summary>
    /// <param name="compilationModel">The compilation model used to generate the source.</param>
    /// <param name="addSource">
    /// An action to add the generated source code. The first parameter is the file name and the second parameter is the source text.
    /// </param>
    /// <param name="reportError">An action to report any exceptions that occur during generation.</param>
    /// <param name="FileName">The base file name of the Scriban template (without extension).</param>
    /// <exception cref="Exception">
    /// Thrown when an error occurs during template parsing or rendering.
    /// </exception>
    /// <remarks>
    /// This method uses a stable unique identifier based on the compilation model to ensure that each generated file is unique.
    /// </remarks>
    internal static void Generate(ICompilationModel compilationModel, Action<string, SourceText> addSource, Action<Exception> reportError, string FileName)
    {
        try
        {
            var file = @$"resources/{FileName}.scriban";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);

            ScriptObject scriptObject = [];
            scriptObject.Import(compilationModel, renamer: MemberRenamer);

            TemplateContext context = new()
            {
                MemberRenamer = MemberRenamer,
                LoopLimit = 0,
                LoopLimitQueryable = 0
            };

            context.PushGlobal(scriptObject);
            var output = template.Render(context);

            // Use a stable unique identifier instead of unpredictable GetHashCode()
            var generatedFileName = $@"{compilationModel.GeneratedFileName}.g.cs";
            addSource(generatedFileName, SourceText.From(output, Encoding.UTF8));
        }
        catch (Exception ex)
        {
            reportError(ex);
        }
    }

    /// <summary>
    /// Renames a member by returning its original name.
    /// </summary>
    /// <param name="member">The member info.</param>
    /// <returns>The original name of the member.</returns>
    private static string MemberRenamer(System.Reflection.MemberInfo member)
    {
        return member.Name;
    }
}
