using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Owasp.Analyzers.Taint;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A05;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InjectionAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor SqlInjectionRule = new("OWASPA05001", "SQL Injection",
        "User-controlled data flows into SQL command without parameterization",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor CommandInjectionRule = new("OWASPA05002", "OS Command Injection",
        "User-controlled data flows into a process command",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor PathTraversalRule = new("OWASPA05003", "Path Traversal",
        "User-controlled data flows into a file system path",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor LdapInjectionRule = new("OWASPA05004", "LDAP Injection",
        "User-controlled data flows into an LDAP filter",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor XPathInjectionRule = new("OWASPA05005", "XPath Injection",
        "User-controlled data flows into an XPath query",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor XssRule = new("OWASPA05006", "Cross-Site Scripting (XSS)",
        "User-controlled data is written to the HTTP response without HTML encoding",
        "OWASP.A05", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly Dictionary<TaintSinks.SinkKind, DiagnosticDescriptor> Rules = new()
    {
        [TaintSinks.SinkKind.SqlInjection] = SqlInjectionRule,
        [TaintSinks.SinkKind.CommandInjection] = CommandInjectionRule,
        [TaintSinks.SinkKind.PathTraversal] = PathTraversalRule,
        [TaintSinks.SinkKind.LdapInjection] = LdapInjectionRule,
        [TaintSinks.SinkKind.XPathInjection] = XPathInjectionRule,
        [TaintSinks.SinkKind.Xss] = XssRule,
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [SqlInjectionRule, CommandInjectionRule, PathTraversalRule, LdapInjectionRule, XPathInjectionRule, XssRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSemanticModelAction(Analyze);
    }

    private static void Analyze(SemanticModelAnalysisContext context)
    {
        var engine = new TaintEngine(context.SemanticModel);
        engine.Analyze(context.SemanticModel.SyntaxTree.GetRoot(context.CancellationToken));

        for (var i = 0; i < engine.SinkHits.Count; i++)
        {
            var kind = engine.SinkHits[i];
            // SSRF and LogInjection are handled by the A01 and A09 analyzers
            if (kind is TaintSinks.SinkKind.Ssrf or TaintSinks.SinkKind.LogInjection) continue;
            if (!Rules.TryGetValue(kind, out var rule)) continue;
            context.ReportDiagnostic(Diagnostic.Create(rule, engine.SinkLocations[i]));
        }
    }
}
