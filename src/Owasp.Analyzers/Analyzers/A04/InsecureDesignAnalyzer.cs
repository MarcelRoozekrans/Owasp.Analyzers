using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A04;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InsecureDesignAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA04002",
        "Missing rate limiting on auth endpoint",
        "Authentication endpoint '{0}' has no rate limiting — susceptible to brute force",
        "OWASP.A04", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly HashSet<string> AuthIndicators = new(StringComparer.OrdinalIgnoreCase)
        { "login", "signin", "authenticate", "token", "auth" };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule002];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var name = method.Identifier.Text;

        if (!AuthIndicators.Any(indicator => name.Contains(indicator, StringComparison.OrdinalIgnoreCase)))
            return;

        var attrNames = method.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString())
            .ToList();

        if (!attrNames.Any(a => a is "HttpPost" or "HttpGet")) return;
        if (attrNames.Any(a => a.Contains("RateLimit"))) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule002, method.Identifier.GetLocation(), name));
    }
}
