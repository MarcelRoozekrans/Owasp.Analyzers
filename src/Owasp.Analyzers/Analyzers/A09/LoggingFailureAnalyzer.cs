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
        "Log injection via tainted user input",
        "Tainted user input flows into a logging call — sanitize input before logging to prevent log injection",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA09002",
        "Sensitive data in log message",
        "String literal passed to a logging method contains sensitive keyword '{0}' — avoid logging passwords, tokens, or secrets",
        "OWASP.A09", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly ImmutableArray<string> SensitiveKeywords =
    [
        "password", "passwd", "secret", "token", "apikey", "api_key",
        "credential", "private_key", "privatekey", "ssn", "creditcard", "cvv"
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocationForSensitiveLog, SyntaxKind.InvocationExpression);
        context.RegisterSemanticModelAction(AnalyzeTaintedLogging);
    }

    private static void AnalyzeInvocationForSensitiveLog(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check if this is a logging method call
        var methodName = LoggingHeuristics.GetInvokedMethodName(invocation);
        if (methodName == null || !LoggingHeuristics.LoggingMethodNames.Contains(methodName))
            return;

        // OWASPA09002 — check string literal arguments for sensitive keywords
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
                        var matchedKeyword = keyword;
                        context.ReportDiagnostic(Diagnostic.Create(Rule002, literal.GetLocation(), matchedKeyword));
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
                context.ReportDiagnostic(Diagnostic.Create(Rule001, engine.SinkLocations[i]));
        }
    }
}
