using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Owasp.Analyzers.Taint;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A03;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InjectionAnalyzer : DiagnosticAnalyzer
{
    private static readonly Dictionary<TaintSinks.SinkKind, DiagnosticDescriptor> Rules = new()
    {
        [TaintSinks.SinkKind.SqlInjection] = new("OWASPA03001", "SQL Injection",
            "User-controlled data flows into SQL command without parameterization",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
        [TaintSinks.SinkKind.CommandInjection] = new("OWASPA03002", "OS Command Injection",
            "User-controlled data flows into a process command",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
        [TaintSinks.SinkKind.PathTraversal] = new("OWASPA03003", "Path Traversal",
            "User-controlled data flows into a file system path",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
        [TaintSinks.SinkKind.LdapInjection] = new("OWASPA03004", "LDAP Injection",
            "User-controlled data flows into an LDAP filter",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
        [TaintSinks.SinkKind.XPathInjection] = new("OWASPA03005", "XPath Injection",
            "User-controlled data flows into an XPath query",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
        [TaintSinks.SinkKind.Xss] = new("OWASPA03006", "Cross-Site Scripting (XSS)",
            "User-controlled data is written to the HTTP response without HTML encoding",
            "OWASP.A03", DiagnosticSeverity.Error, isEnabledByDefault: true),
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [.. Rules.Values];

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
            // SSRF and LogInjection are handled by A10/A09 analyzers
            if (kind is TaintSinks.SinkKind.Ssrf or TaintSinks.SinkKind.LogInjection) continue;
            if (!Rules.TryGetValue(kind, out var rule)) continue;
            context.ReportDiagnostic(Diagnostic.Create(rule, engine.SinkLocations[i]));
        }
    }
}
