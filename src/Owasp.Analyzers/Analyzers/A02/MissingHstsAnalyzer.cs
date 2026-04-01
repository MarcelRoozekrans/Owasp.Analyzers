using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A02;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingHstsAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new("OWASPA02008",
        "Missing HSTS middleware",
        "app.UseHsts() is not called — HTTP Strict Transport Security is not enforced",
        "OWASP.A02", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Identifier.Text is not ("Configure" or "ConfigureApp")) return;
        if (method.Body == null) return;

        var calls = method.Body.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(i => i.Expression.ToString())
            .ToList();

        if (!calls.Any(c => c.Contains("UseHsts")))
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation()));
    }
}
