using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Tests;

public static class AnalyzerTestHelper
{
    /// <summary>
    /// Runs an analyzer against the given C# code and returns all diagnostics.
    /// Pass additionalReferences for code that uses ASP.NET Core, System.Data, etc.
    /// </summary>
    public static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string code,
        DiagnosticAnalyzer analyzer,
        IEnumerable<MetadataReference>? additionalReferences = null)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        if (additionalReferences != null)
            references.AddRange(additionalReferences);

        var compilation = CSharpCompilation.Create("TestAssembly")
            .AddReferences(references)
            .AddSyntaxTrees(tree);

        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create(analyzer));

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    /// <summary>
    /// Asserts exactly one diagnostic with the given ID was reported (ignores other IDs).
    /// </summary>
    public static Diagnostic AssertSingleDiagnostic(ImmutableArray<Diagnostic> diagnostics, string expectedId)
    {
        var matching = diagnostics.Where(d => d.Id == expectedId).ToList();
        Assert.Single(matching);
        return matching[0];
    }

    /// <summary>
    /// Asserts no diagnostics were reported.
    /// </summary>
    public static void AssertNoDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        Assert.Empty(diagnostics);
    }
}
