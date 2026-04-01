using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Owasp.Analyzers.Taint;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A10;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SsrfAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA10001",
        "Server-Side Request Forgery via HttpClient",
        "Tainted user input flows into HttpClient request URL — validate or allowlist the URL before use",
        "OWASP.A10", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA10002",
        "Server-Side Request Forgery via WebClient/WebRequest",
        "Tainted user input flows into WebClient or WebRequest URL — validate or allowlist the URL before use",
        "OWASP.A10", DiagnosticSeverity.Error, isEnabledByDefault: true);

    // Rule003 is a linter-style rule: it fires unconditionally on AllowAutoRedirect = true assignments
    // without taint-flow context. Warning (not Error) severity allows teams to suppress where legitimate.
    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA10003",
        "AllowAutoRedirect enabled",
        "AllowAutoRedirect is set to true — this can allow attackers to redirect requests to internal services",
        "OWASP.A10", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    // HttpClient sinks registered in TaintSinks: GetAsync, PostAsync, SendAsync
    private static readonly ImmutableHashSet<string> HttpClientSinkMethods = ImmutableHashSet.Create(
        "GetAsync", "PostAsync", "SendAsync");

    // WebClient / WebRequest sinks registered in TaintSinks: DownloadString, UploadString, Create
    private static readonly ImmutableHashSet<string> WebSinkMethods = ImmutableHashSet.Create(
        "DownloadString", "UploadString", "Create");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002, Rule003];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSemanticModelAction(AnalyzeTaintedSsrf);
        context.RegisterSyntaxNodeAction(AnalyzeAllowAutoRedirect, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void AnalyzeTaintedSsrf(SemanticModelAnalysisContext context)
    {
        var engine = new TaintEngine(context.SemanticModel);
        engine.Analyze(context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken));

        for (var i = 0; i < engine.SinkHits.Count; i++)
        {
            if (engine.SinkHits[i] != TaintSinks.SinkKind.Ssrf) continue;

            // Determine which rule to fire based on which sink was hit
            var location = engine.SinkLocations[i];
            var rule = DetermineRule(location);
            context.ReportDiagnostic(Diagnostic.Create(rule, location));
        }
    }

    private static DiagnosticDescriptor DetermineRule(Location location)
    {
        // Walk up from the location's node to find the invocation expression,
        // then check if the method name belongs to HttpClient or WebClient/WebRequest sinks.
        var node = location.SourceTree?.GetRoot()
            .FindNode(location.SourceSpan);

        // Traverse ancestors to find the invocation
        var invocation = node?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
        if (invocation?.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.Text;
            if (HttpClientSinkMethods.Contains(methodName))
                return Rule001;
            if (WebSinkMethods.Contains(methodName))
                return Rule002;
        }

        // Fallback to Rule001: covers HttpClient.SendAsync hits (which use Rule001) and any
        // case where the invocation expression cannot be resolved to a known sink method.
        return Rule001;
    }

    private static void AnalyzeAllowAutoRedirect(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        // LHS must contain "AllowAutoRedirect"
        var lhsText = assignment.Left.ToString();
        if (!lhsText.Contains("AllowAutoRedirect"))
            return;

        // RHS must be the literal `true`
        if (assignment.Right is LiteralExpressionSyntax literal &&
            literal.IsKind(SyntaxKind.TrueLiteralExpression))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule003, assignment.GetLocation()));
        }
    }
}
