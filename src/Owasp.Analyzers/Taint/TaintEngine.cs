using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Owasp.Analyzers.Taint;

/// <summary>
/// Intra-method taint analysis engine.
/// Tracks user-controlled data from ASP.NET sources through assignments and string
/// operations to dangerous sinks. Scope: single method body only (v1).
/// </summary>
internal sealed class TaintEngine
{
    private readonly SemanticModel _model;

    /// <summary>Names of local variables currently holding tainted data.</summary>
    public HashSet<string> TaintedLocals { get; } = new(StringComparer.Ordinal);

    /// <summary>Sink kinds discovered during analysis (parallel with SinkLocations).</summary>
    public List<TaintSinks.SinkKind> SinkHits { get; } = [];

    /// <summary>Locations of sink hits (parallel with SinkHits).</summary>
    public List<Location> SinkLocations { get; } = [];

    private static readonly HashSet<string> SanitizerMethods = new(StringComparer.Ordinal)
    {
        "HtmlEncode", "UrlEncode", "EscapeDataString", "Encode",
        "HtmlAttributeEncode", "JavaScriptStringEncode"
    };

    public TaintEngine(SemanticModel model) => _model = model;

    public void Analyze(SyntaxNode root)
    {
        // Process each method body independently to prevent cross-method taint leakage.
        // For each method, reset TaintedLocals and walk its body statements recursively.
        foreach (var method in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (method.Body == null) continue;
            TaintedLocals.Clear();
            foreach (var statement in method.Body.DescendantNodes().OfType<StatementSyntax>())
                AnalyzeStatement(statement);
        }
    }

    private void AnalyzeStatement(StatementSyntax statement)
    {
        switch (statement)
        {
            case LocalDeclarationStatementSyntax local:
                AnalyzeLocalDeclaration(local);
                break;
            case ExpressionStatementSyntax expr:
                AnalyzeExpressionStatement(expr);
                break;
        }
    }

    private void AnalyzeLocalDeclaration(LocalDeclarationStatementSyntax local)
    {
        foreach (var variable in local.Declaration.Variables)
        {
            if (variable.Initializer?.Value is not { } initializer) continue;

            // Check if the initializer is a sink invocation with tainted args
            if (initializer is InvocationExpressionSyntax invocation)
                AnalyzeInvocation(invocation);

            if (IsTainted(initializer))
                TaintedLocals.Add(variable.Identifier.Text);
        }
    }

    private void AnalyzeExpressionStatement(ExpressionStatementSyntax expr)
    {
        if (expr.Expression is AssignmentExpressionSyntax assignment && IsTainted(assignment.Right))
        {
            switch (assignment.Left)
            {
                // Local variable re-assignment: x = tainted
                case IdentifierNameSyntax id:
                    TaintedLocals.Add(id.Identifier.Text);
                    break;

                // Property sink: command.CommandText = tainted
                case MemberAccessExpressionSyntax memberAccess:
                    var symbol = _model.GetSymbolInfo(memberAccess).Symbol;
                    if (symbol != null)
                    {
                        var sinkKind = TaintSinks.GetSinkKind(symbol);
                        if (sinkKind.HasValue)
                        {
                            SinkHits.Add(sinkKind.Value);
                            SinkLocations.Add(assignment.GetLocation());
                        }
                    }
                    break;
            }
        }

        // Method sink: client.GetAsync(tainted)
        if (expr.Expression is InvocationExpressionSyntax invocation)
            AnalyzeInvocation(invocation);
    }

    private void AnalyzeInvocation(InvocationExpressionSyntax invocation)
    {
        var symbol = _model.GetSymbolInfo(invocation).Symbol;
        if (symbol == null) return;

        var args = invocation.ArgumentList.Arguments;
        for (var i = 0; i < args.Count; i++)
        {
            if (!IsTainted(args[i].Expression)) continue;

            var sinkKind = TaintSinks.GetSinkKind(symbol, i);
            if (sinkKind.HasValue)
            {
                SinkHits.Add(sinkKind.Value);
                SinkLocations.Add(invocation.GetLocation());
            }
        }
    }

    /// <summary>
    /// Returns true if the given expression carries tainted (user-controlled) data.
    /// </summary>
    internal bool IsTainted(ExpressionSyntax expression) => expression switch
    {
        // Local variable reference — tainted if previously marked
        IdentifierNameSyntax id =>
            TaintedLocals.Contains(id.Identifier.Text),

        // Request.Query["id"] — element access on a tainted receiver
        ElementAccessExpressionSyntax el =>
            IsTainted(el.Expression),

        // Request.Query — direct source access
        MemberAccessExpressionSyntax ma =>
            TaintSources.IsRequestAccess(ma) || IsTainted(ma.Expression),

        // $"...{tainted}..." — any tainted interpolation hole
        InterpolatedStringExpressionSyntax interp =>
            interp.Contents.OfType<InterpolationSyntax>().Any(i => IsTainted(i.Expression)),

        // "prefix" + tainted — binary string concat
        BinaryExpressionSyntax { RawKind: (int)SyntaxKind.AddExpression } bin =>
            IsTainted(bin.Left) || IsTainted(bin.Right),

        // Method call — tainted if any arg is tainted and not a sanitizer
        InvocationExpressionSyntax inv =>
            !IsSanitizerCall(inv) &&
            inv.ArgumentList.Arguments.Any(a => IsTainted(a.Expression)),

        _ => false
    };

    private static bool IsSanitizerCall(InvocationExpressionSyntax invocation)
    {
        var methodName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax m => m.Name.Identifier.Text,
            IdentifierNameSyntax i => i.Identifier.Text,
            _ => string.Empty
        };
        return SanitizerMethods.Contains(methodName);
    }
}
