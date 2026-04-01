using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Tests;

public static class AnalyzerTestHelper
{
    /// <summary>
    /// Runs an analyzer against the given C# code snippet and returns all diagnostics.
    /// The code is wrapped in a minimal compilable class context.
    /// </summary>
    public static ImmutableArray<Diagnostic> GetDiagnostics(string code, DiagnosticAnalyzer analyzer)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("TestAssembly")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(tree);

        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create(analyzer));

        return compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
    }

    /// <summary>
    /// Asserts exactly one diagnostic with the given ID was reported.
    /// </summary>
    public static Diagnostic AssertSingleDiagnostic(ImmutableArray<Diagnostic> diagnostics, string expectedId)
    {
        Assert.Single(diagnostics);
        Assert.Equal(expectedId, diagnostics[0].Id);
        return diagnostics[0];
    }

    /// <summary>
    /// Asserts no diagnostics were reported.
    /// </summary>
    public static void AssertNoDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        Assert.Empty(diagnostics);
    }
}
