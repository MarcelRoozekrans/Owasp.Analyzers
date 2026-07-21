using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A10;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionHandlingAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA10001",
        "Empty catch block suppresses exceptions silently",
        "Empty catch block swallows exceptions without any logging or handling — add logging or rethrow",
        "OWASP.A10", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA10002",
        "Catch block missing logging",
        "Catch block does not contain any logging call — exceptions should be logged for security monitoring",
        "OWASP.A10", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule001, Rule002];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;
        var block = catchClause.Block;
        var statements = block.Statements;

        // OWASPA10001 — empty catch block (zero statements, ignoring trivia/comments)
        if (statements.Count == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule001, catchClause.GetLocation()));
            return;
        }

        // OWASPA10002 — non-empty catch with no logging invocation
        if (!BlockContainsLoggingCall(block))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule002, catchClause.GetLocation()));
        }
    }

    private static bool BlockContainsLoggingCall(BlockSyntax block)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var methodName = LoggingHeuristics.GetInvokedMethodName(invocation);
            if (methodName != null && LoggingHeuristics.LoggingMethodNames.Contains(methodName))
                return true;
        }
        return false;
    }
}
