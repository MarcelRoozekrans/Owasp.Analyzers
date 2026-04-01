using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A01;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HardcodedRoleAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId002 = "OWASPA01002";
    public const string DiagnosticId003 = "OWASPA01003";

    private static readonly DiagnosticDescriptor Rule002 = new(
        DiagnosticId002, "Hardcoded role in [Authorize]",
        "Hardcoded role string '{0}' in [Authorize] attribute — use constants or policy names",
        "OWASP.A01", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new(
        DiagnosticId003, "Hardcoded string in IsInRole",
        "Hardcoded role string in User.IsInRole() — use constants",
        "OWASP.A01", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule002, Rule003];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attr = (AttributeSyntax)context.Node;
        if (attr.Name.ToString() is not ("Authorize" or "AuthorizeAttribute")) return;

        var rolesArg = attr.ArgumentList?.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.Identifier.Text == "Roles");
        if (rolesArg?.Expression is LiteralExpressionSyntax literal)
            context.ReportDiagnostic(Diagnostic.Create(Rule002, literal.GetLocation(), literal.Token.ValueText));
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;
        if (memberAccess.Name.Identifier.Text != "IsInRole") return;

        var arg = invocation.ArgumentList.Arguments.FirstOrDefault();
        if (arg?.Expression is LiteralExpressionSyntax)
            context.ReportDiagnostic(Diagnostic.Create(Rule003, invocation.GetLocation()));
    }
}
