using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A05;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecurityMisconfigAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA05001",
        "Developer exception page in production",
        "UseDeveloperExceptionPage() should only be called in Development environment — wrap in IsDevelopment() check",
        "OWASP.A05", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA05002",
        "Missing HTTPS redirection",
        "app.UseHttpsRedirection() is not called — HTTP requests will not be redirected to HTTPS",
        "OWASP.A05", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA05003",
        "Directory browsing enabled",
        "UseDirectoryBrowser() exposes directory listings — remove unless intentional",
        "OWASP.A05", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule004 = new("OWASPA05004",
        "Error details exposed",
        "IncludeErrorDetails is enabled — stack traces and internal details may be exposed to clients",
        "OWASP.A05", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule005 = new("OWASPA05005",
        "Antiforgery not configured",
        "services.AddAntiforgery() is not called — CSRF protection may not be active",
        "OWASP.A05", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule006 = new("OWASPA05006",
        "Hardcoded credential",
        "Variable '{0}' appears to contain a hardcoded credential",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly HashSet<string> CredentialIndicators = new(StringComparer.OrdinalIgnoreCase)
        { "password", "passwd", "pwd", "credential", "secret", "apikey", "api_key" };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002, Rule003, Rule004, Rule005, Rule006];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMethodForMissingCalls, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var methodName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax m => m.Name.Identifier.Text,
            IdentifierNameSyntax i => i.Identifier.Text,
            _ => string.Empty
        };

        switch (methodName)
        {
            case "UseDeveloperExceptionPage":
                if (!IsInsideEnvironmentCheck(invocation))
                    context.ReportDiagnostic(Diagnostic.Create(Rule001, invocation.GetLocation()));
                break;
            case "UseDirectoryBrowser":
                context.ReportDiagnostic(Diagnostic.Create(Rule003, invocation.GetLocation()));
                break;
        }
    }

    private static void AnalyzeMethodForMissingCalls(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Body == null) return;

        var calls = method.Body.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(i => i.Expression.ToString())
            .ToList();

        if (method.Identifier.Text is "Configure" or "ConfigureApp")
        {
            if (!calls.Any(c => c.Contains("UseHttpsRedirection")))
                context.ReportDiagnostic(Diagnostic.Create(Rule002, method.Identifier.GetLocation()));
        }

        if (method.Identifier.Text is "ConfigureServices" or "AddServices" or "AddApplicationServices")
        {
            if (!calls.Any(c => c.Contains("AddAntiforgery")))
                context.ReportDiagnostic(Diagnostic.Create(Rule005, method.Identifier.GetLocation()));
        }
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        var leftText = assignment.Left.ToString();

        if ((leftText.Contains("IncludeErrorDetails") || leftText.Contains("IncludeErrorDetailPolicy")) &&
            assignment.Right.IsKind(SyntaxKind.TrueLiteralExpression))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule004, assignment.GetLocation()));
        }

        // Hardcoded credential in assignment
        CheckForHardcodedCredential(context, leftText, assignment.Right, assignment.GetLocation());
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var local = (LocalDeclarationStatementSyntax)context.Node;
        foreach (var variable in local.Declaration.Variables)
        {
            if (variable.Initializer?.Value is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                CheckForHardcodedCredential(context, variable.Identifier.Text,
                    variable.Initializer.Value, variable.Initializer.Value.GetLocation());
        }
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        foreach (var variable in field.Declaration.Variables)
        {
            if (variable.Initializer?.Value is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                CheckForHardcodedCredential(context, variable.Identifier.Text,
                    variable.Initializer.Value, variable.Initializer.Value.GetLocation());
        }
    }

    /// <summary>
    /// Checks for hardcoded credentials in variable/field initializers and assignments (OWASPA05006).
    /// Detection scope: general credential variable names (password, passwd, pwd, credential, secret, apikey, api_key).
    /// This is intentionally separate from OWASPA02004, which targets cryptographic key/IV byte[] literals.
    /// </summary>
    private static void CheckForHardcodedCredential(SyntaxNodeAnalysisContext context,
        string name, ExpressionSyntax value, Location location)
    {
        if (value is not LiteralExpressionSyntax literal) return;
        if (!literal.IsKind(SyntaxKind.StringLiteralExpression)) return;
        if (string.IsNullOrEmpty(literal.Token.ValueText)) return;

        var nameLower = name.ToLowerInvariant();
        if (CredentialIndicators.Any(k => nameLower.Contains(k)))
            context.ReportDiagnostic(Diagnostic.Create(Rule006, location, name));
    }

    private static bool IsInsideEnvironmentCheck(SyntaxNode node)
    {
        var parent = node.Parent;
        while (parent != null)
        {
            if (parent is IfStatementSyntax ifStmt)
            {
                var condition = ifStmt.Condition.ToString();
                if (condition.Contains("IsDevelopment") ||
                    condition.Contains("IsEnvironment") ||
                    condition.Contains("ASPNETCORE_ENVIRONMENT"))
                    return true;
            }
            parent = parent.Parent;
        }
        return false;
    }
}
