using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A02;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WeakCryptoAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA02001",
        "Weak cryptographic algorithm",
        "'{0}' is a weak or broken algorithm — use AES-GCM, SHA-256 or stronger",
        "OWASP.A02", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA02002",
        "ECB cipher mode is insecure",
        "CipherMode.ECB does not provide semantic security — use AES-GCM or AES-CBC with HMAC",
        "OWASP.A02", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA02003",
        "System.Random is not cryptographically secure",
        "Use RandomNumberGenerator.GetBytes() instead of System.Random for security-sensitive operations",
        "OWASP.A02", DiagnosticSeverity.Info, isEnabledByDefault: true);

    private static readonly HashSet<string> WeakAlgorithms = new(StringComparer.Ordinal)
    {
        "MD5", "SHA1", "SHA1Managed", "MD5CryptoServiceProvider",
        "DES", "DESCryptoServiceProvider", "RC2", "RC2CryptoServiceProvider",
        "TripleDES", "TripleDESCryptoServiceProvider"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule001, Rule002, Rule003];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax { Name.Identifier.Text: "Create" } memberAccess)
            return;
        var typeName = memberAccess.Expression.ToString();
        if (WeakAlgorithms.Contains(typeName))
            context.ReportDiagnostic(Diagnostic.Create(Rule001, invocation.GetLocation(), typeName));
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;
        var typeName = creation.Type.ToString();
        if (WeakAlgorithms.Contains(typeName))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule001, creation.GetLocation(), typeName));
            return;
        }
        if (typeName == "Random")
            context.ReportDiagnostic(Diagnostic.Create(Rule003, creation.GetLocation()));
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        if (assignment.Right is MemberAccessExpressionSyntax { Name.Identifier.Text: "ECB" } right &&
            right.Expression.ToString() == "CipherMode")
            context.ReportDiagnostic(Diagnostic.Create(Rule002, assignment.GetLocation()));
    }
}
