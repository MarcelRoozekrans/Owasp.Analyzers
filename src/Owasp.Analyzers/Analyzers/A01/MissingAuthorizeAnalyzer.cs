using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A01;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingAuthorizeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "OWASPA01001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Controller action missing authorization",
        "Action '{0}' is not decorated with [Authorize] or [AllowAnonymous]",
        "OWASP.A01",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "OWASP A01: Broken Access Control — every controller action must explicitly declare its authorization requirement.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(method);
        if (symbol is not IMethodSymbol methodSymbol) return;

        if (methodSymbol.DeclaredAccessibility != Accessibility.Public) return;

        // Check controller type — try both semantic and syntax-based checks
        if (!IsControllerType(methodSymbol.ContainingType, method)) return;

        // Require an HTTP method attribute on the method (syntax-based)
        if (!HasHttpMethodAttributeSyntax(method)) return;

        // Check authorize/allow-anonymous on method (syntax-based)
        if (HasAuthorizeOrAllowAnonymousSyntax(method.AttributeLists)) return;

        // Check authorize/allow-anonymous on the containing class (syntax-based)
        if (method.Parent is TypeDeclarationSyntax classDecl &&
            HasAuthorizeOrAllowAnonymousSyntax(classDecl.AttributeLists)) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), methodSymbol.Name));
    }

    private static bool IsControllerType(INamedTypeSymbol type, MethodDeclarationSyntax method)
    {
        // Semantic check: base class name
        var current = type.BaseType;
        while (current != null)
        {
            if (current.Name is "Controller" or "ControllerBase") return true;
            current = current.BaseType;
        }

        // Semantic check: attributes (may not resolve in tests)
        if (type.GetAttributes().Any(a => a.AttributeClass?.Name is "ApiControllerAttribute" or "ControllerAttribute"))
            return true;

        // Syntax fallback: check [ApiController] or [Controller] on the class
        if (method.Parent is TypeDeclarationSyntax classDecl)
        {
            if (HasAttributeNameSyntax(classDecl.AttributeLists, "ApiController", "ApiControllerAttribute",
                    "Controller", "ControllerAttribute"))
                return true;

            // Syntax fallback: check if base type name is Controller or ControllerBase
            if (classDecl.BaseList?.Types.Any(t => t.Type.ToString() is "Controller" or "ControllerBase") == true)
                return true;
        }

        return false;
    }

    private static bool HasHttpMethodAttributeSyntax(MethodDeclarationSyntax method) =>
        HasAttributeNameSyntax(method.AttributeLists,
            "HttpGet", "HttpGetAttribute",
            "HttpPost", "HttpPostAttribute",
            "HttpPut", "HttpPutAttribute",
            "HttpDelete", "HttpDeleteAttribute",
            "HttpPatch", "HttpPatchAttribute",
            "Route", "RouteAttribute");

    private static bool HasAuthorizeOrAllowAnonymousSyntax(SyntaxList<AttributeListSyntax> attributeLists) =>
        HasAttributeNameSyntax(attributeLists,
            "Authorize", "AuthorizeAttribute",
            "AllowAnonymous", "AllowAnonymousAttribute");

    private static bool HasAttributeNameSyntax(SyntaxList<AttributeListSyntax> attributeLists, params string[] names)
    {
        var nameSet = new HashSet<string>(names, StringComparer.Ordinal);
        return attributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => nameSet.Contains(a.Name.ToString()));
    }
}
