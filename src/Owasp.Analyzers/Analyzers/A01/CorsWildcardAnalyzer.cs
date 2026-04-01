using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A01;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CorsWildcardAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "OWASPA01004";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId, "CORS wildcard allows any origin",
        "AllowAnyOrigin() permits requests from any domain — restrict to known origins",
        "OWASP.A01", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "AllowAnyOrigin" })
            context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
    }
}
