using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A01;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingAntiforgeryTokenAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "OWASPA01005";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId, "Missing [ValidateAntiForgeryToken]",
        "State-changing action '{0}' is missing [ValidateAntiForgeryToken] — susceptible to CSRF",
        "OWASP.A01", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly HashSet<string> StateChangingVerbs =
        ["HttpPost", "HttpPostAttribute", "HttpPut", "HttpPutAttribute",
         "HttpDelete", "HttpDeleteAttribute", "HttpPatch", "HttpPatchAttribute"];

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
        var syntaxAttrNames = method.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString())
            .ToList();

        if (!syntaxAttrNames.Any(StateChangingVerbs.Contains)) return;

        if (syntaxAttrNames.Any(n => n is "ValidateAntiForgeryToken" or "ValidateAntiForgeryTokenAttribute")) return;
        if (syntaxAttrNames.Any(n => n is "IgnoreAntiforgeryToken" or "IgnoreAntiforgeryTokenAttribute")) return;

        // Also skip if the containing class has [AutoValidateAntiforgeryToken]
        if (method.Parent is TypeDeclarationSyntax typeDecl)
        {
            var classAttrNames = typeDecl.AttributeLists
                .SelectMany(al => al.Attributes)
                .Select(a => a.Name.ToString())
                .ToList();
            if (classAttrNames.Any(n => n is "AutoValidateAntiforgeryToken" or "AutoValidateAntiforgeryTokenAttribute"))
                return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text));
    }
}
