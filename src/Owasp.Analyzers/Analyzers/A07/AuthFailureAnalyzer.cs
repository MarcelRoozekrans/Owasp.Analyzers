using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A07;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AuthFailureAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA07001",
        "JWT algorithm bypass via SecurityAlgorithms.None",
        "SecurityAlgorithms.None disables signature verification — use a secure algorithm such as HmacSha256",
        "OWASP.A07", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA07002",
        "Token lifetime validation disabled",
        "ValidateLifetime = false in TokenValidationParameters — expired tokens will be accepted",
        "OWASP.A07", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA07003",
        "Token signing key validation disabled",
        "ValidateIssuerSigningKey = false in TokenValidationParameters — tampered tokens may be accepted",
        "OWASP.A07", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule004 = new("OWASPA07004",
        "CookieOptions missing HttpOnly or Secure flag",
        "CookieOptions should set both HttpOnly = true and Secure = true to protect session cookies",
        "OWASP.A07", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule005 = new("OWASPA07005",
        "SameSite=None cookie without Secure flag",
        "CookieOptions with SameSite = SameSiteMode.None must also set Secure = true",
        "OWASP.A07", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002, Rule003, Rule004, Rule005];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;

        // Detect SecurityAlgorithms.None
        if (memberAccess.Expression.ToString() == "SecurityAlgorithms" &&
            memberAccess.Name.Identifier.Text == "None")
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule001, memberAccess.GetLocation()));
        }
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;
        var typeName = creation.Type.ToString();

        if (typeName == "TokenValidationParameters")
        {
            AnalyzeTokenValidationParameters(context, creation);
        }
        else if (typeName == "CookieOptions")
        {
            AnalyzeCookieOptions(context, creation);
        }
    }

    private static void AnalyzeTokenValidationParameters(
        SyntaxNodeAnalysisContext context,
        ObjectCreationExpressionSyntax creation)
    {
        if (creation.Initializer == null) return;

        var props = GetInitializerProperties(creation.Initializer);

        if (props.TryGetValue("ValidateLifetime", out var lifetimeValue) &&
            lifetimeValue.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule002, creation.GetLocation()));
        }

        if (props.TryGetValue("ValidateIssuerSigningKey", out var signingKeyValue) &&
            signingKeyValue.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule003, creation.GetLocation()));
        }
    }

    private static void AnalyzeCookieOptions(
        SyntaxNodeAnalysisContext context,
        ObjectCreationExpressionSyntax creation)
    {
        if (creation.Initializer == null)
        {
            // No initializer at all — both flags missing
            context.ReportDiagnostic(Diagnostic.Create(Rule004, creation.GetLocation()));
            return;
        }

        var props = GetInitializerProperties(creation.Initializer);

        // Check OWASPA07004: HttpOnly and Secure must both be true
        bool httpOnlySet = props.TryGetValue("HttpOnly", out var httpOnlyVal) &&
                           httpOnlyVal.Equals("true", StringComparison.OrdinalIgnoreCase);
        bool secureSet = props.TryGetValue("Secure", out var secureVal) &&
                         secureVal.Equals("true", StringComparison.OrdinalIgnoreCase);

        if (!httpOnlySet || !secureSet)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule004, creation.GetLocation()));
        }

        // Check OWASPA07005: SameSite=None requires Secure=true
        if (props.TryGetValue("SameSite", out var sameSiteVal) &&
            sameSiteVal.Contains("None") &&
            !secureSet)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule005, creation.GetLocation()));
        }
    }

    /// <summary>
    /// Returns a dictionary mapping property name to value text from an object initializer expression.
    /// </summary>
    private static Dictionary<string, string> GetInitializerProperties(InitializerExpressionSyntax initializer)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var expression in initializer.Expressions)
        {
            if (expression is AssignmentExpressionSyntax assignment &&
                assignment.Left is IdentifierNameSyntax propertyName)
            {
                result[propertyName.Identifier.Text] = assignment.Right.ToString();
            }
        }

        return result;
    }
}
