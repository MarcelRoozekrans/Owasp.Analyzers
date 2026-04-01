using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A02;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InsecureTlsAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule005 = new("OWASPA02005",
        "Legacy TLS protocol",
        "SecurityProtocol includes deprecated Ssl3 or Tls — use Tls12 or Tls13",
        "OWASP.A02", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule006 = new("OWASPA02006",
        "Certificate validation disabled",
        "ServerCertificateValidationCallback always returns true — TLS verification is disabled",
        "OWASP.A02", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule007 = new("OWASPA02007",
        "Hardcoded HTTP URL",
        "URL '{0}' uses HTTP — use HTTPS to protect data in transit",
        "OWASP.A02", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule005, Rule006, Rule007];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeLiteral, SyntaxKind.StringLiteralExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        var leftText = assignment.Left.ToString();
        var rightText = assignment.Right.ToString();

        if (leftText.Contains("SecurityProtocol"))
        {
            if (rightText.Contains("Ssl3") ||
                (rightText.Contains("Tls") && !rightText.Contains("Tls12") && !rightText.Contains("Tls13")))
                context.ReportDiagnostic(Diagnostic.Create(Rule005, assignment.GetLocation()));
        }

        if (leftText.Contains("ServerCertificateValidationCallback") &&
            rightText.Contains("true") && !rightText.Contains("false"))
            context.ReportDiagnostic(Diagnostic.Create(Rule006, assignment.GetLocation()));
    }

    private static void AnalyzeLiteral(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        var value = literal.Token.ValueText;
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && value.Length > 7)
            context.ReportDiagnostic(Diagnostic.Create(Rule007, literal.GetLocation(), value));
    }
}
