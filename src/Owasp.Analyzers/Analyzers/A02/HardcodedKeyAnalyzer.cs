using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A02;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HardcodedKeyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "OWASPA02004";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId, "Hardcoded cryptographic key",
        "Variable '{0}' appears to contain a hardcoded cryptographic key or IV",
        "OWASP.A02", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly HashSet<string> KeyIndicators = new(StringComparer.OrdinalIgnoreCase)
    {
        "key", "iv", "secret", "password", "salt", "nonce", "hmackey"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var local = (LocalDeclarationStatementSyntax)context.Node;
        foreach (var variable in local.Declaration.Variables)
            CheckVariable(context, variable.Identifier.Text, variable.Initializer?.Value);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        foreach (var variable in field.Declaration.Variables)
            CheckVariable(context, variable.Identifier.Text, variable.Initializer?.Value);
    }

    private static void CheckVariable(SyntaxNodeAnalysisContext context, string name, ExpressionSyntax? initializer)
    {
        if (initializer is not (ArrayCreationExpressionSyntax or ImplicitArrayCreationExpressionSyntax))
            return;
        var nameLower = name.ToLowerInvariant();
        if (KeyIndicators.Any(k => nameLower.Contains(k)))
            context.ReportDiagnostic(Diagnostic.Create(Rule, initializer!.GetLocation(), name));
    }
}
