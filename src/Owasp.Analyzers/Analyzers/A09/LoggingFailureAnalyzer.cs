using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Owasp.Analyzers.Taint;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A09;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LoggingFailureAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA09001",
        "Empty catch block suppresses exceptions silently",
        "Empty catch block swallows exceptions without any logging or handling — add logging or rethrow",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA09002",
        "Catch block missing logging",
        "Catch block does not contain any logging call — exceptions should be logged for security monitoring",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA09003",
        "Log injection via tainted user input",
        "Tainted user input flows into a logging call — sanitize input before logging to prevent log injection",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule004 = new("OWASPA09004",
        "Sensitive data in log message",
        "String literal passed to a logging method contains a sensitive keyword — avoid logging passwords, tokens, or other secrets",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly ImmutableHashSet<string> LoggingMethodNames = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase,
        "Log", "LogInformation", "LogWarning", "LogError", "LogDebug", "LogTrace", "LogCritical",
        "WriteLine", "Write", "Info", "Error", "Warn", "Debug", "Fatal");

    private static readonly ImmutableArray<string> SensitiveKeywords =
    [
        "password", "passwd", "secret", "token", "apikey", "api_key",
        "credential", "private_key", "privatekey", "ssn", "creditcard", "cvv"
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002, Rule003, Rule004];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
        context.RegisterSyntaxNodeAction(AnalyzeInvocationForSensitiveLog, SyntaxKind.InvocationExpression);
        context.RegisterSemanticModelAction(AnalyzeTaintedLogging);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;
        var block = catchClause.Block;
        var statements = block.Statements;

        // OWASPA09001 — empty catch block (zero statements, ignoring trivia/comments)
        if (statements.Count == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule001, catchClause.GetLocation()));
            return;
        }

        // OWASPA09002 — non-empty catch with no logging invocation
        if (!BlockContainsLoggingCall(block))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule002, catchClause.GetLocation()));
        }
    }

    private static bool BlockContainsLoggingCall(BlockSyntax block)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var methodName = GetInvokedMethodName(invocation);
            if (methodName != null && LoggingMethodNames.Contains(methodName))
                return true;
        }
        return false;
    }

    private static string? GetInvokedMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax member => member.Name.Identifier.Text,
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            _ => null
        };
    }

    private static void AnalyzeInvocationForSensitiveLog(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check if this is a logging method call
        var methodName = GetInvokedMethodName(invocation);
        if (methodName == null || !LoggingMethodNames.Contains(methodName))
            return;

        // OWASPA09004 — check string literal arguments for sensitive keywords
        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            if (argument.Expression is LiteralExpressionSyntax literal &&
                literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var value = literal.Token.ValueText;
                foreach (var keyword in SensitiveKeywords)
                {
                    if (value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule004, literal.GetLocation()));
                        return;
                    }
                }
            }
        }
    }

    private static void AnalyzeTaintedLogging(SemanticModelAnalysisContext context)
    {
        var engine = new TaintEngine(context.SemanticModel);
        engine.Analyze(context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken));
        for (var i = 0; i < engine.SinkHits.Count; i++)
        {
            if (engine.SinkHits[i] == TaintSinks.SinkKind.LogInjection)
                context.ReportDiagnostic(Diagnostic.Create(Rule003, engine.SinkLocations[i]));
        }
    }
}
