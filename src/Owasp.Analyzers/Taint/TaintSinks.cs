using Microsoft.CodeAnalysis;

namespace Owasp.Analyzers.Taint;

/// <summary>
/// Identifies dangerous sinks where tainted data causes a vulnerability.
/// </summary>
internal static class TaintSinks
{
    public enum SinkKind
    {
        SqlInjection,
        CommandInjection,
        PathTraversal,
        LdapInjection,
        XPathInjection,
        Xss,
        DynamicLinq,  // placeholder — no sink entries yet; added when Dynamic LINQ rule is implemented
        Ssrf,
        LogInjection
    }

    private record SinkDefinition(string TypeName, string MemberName, int TaintedArgumentIndex, SinkKind Kind);

    // NOTE: Matching is by simple type name only (no namespace). "File" could collide with
    // user-defined types named File. This is an accepted v1 limitation — semantic type
    // resolution is deferred to v2.

    // AllSinks must be declared before SinkIndex so the array is initialized first.
    private static readonly SinkDefinition[] AllSinks =
    [
        // A03 — SQL
        new("SqlCommand", "CommandText", -1, SinkKind.SqlInjection),
        new("NpgsqlCommand", "CommandText", -1, SinkKind.SqlInjection),
        new("MySqlCommand", "CommandText", -1, SinkKind.SqlInjection),

        // A03 — OS command
        new("Process", "Start", 0, SinkKind.CommandInjection),
        new("ProcessStartInfo", "FileName", -1, SinkKind.CommandInjection),
        new("ProcessStartInfo", "Arguments", -1, SinkKind.CommandInjection),

        // A03 — Path traversal
        new("File", "Open", 0, SinkKind.PathTraversal),
        new("File", "ReadAllText", 0, SinkKind.PathTraversal),
        new("File", "WriteAllText", 0, SinkKind.PathTraversal),
        new("File", "ReadAllBytes", 0, SinkKind.PathTraversal),
        new("Directory", "GetFiles", 0, SinkKind.PathTraversal),
        new("Directory", "GetDirectories", 0, SinkKind.PathTraversal),

        // A03 — LDAP
        new("DirectorySearcher", "Filter", -1, SinkKind.LdapInjection),

        // A03 — XPath
        new("XPathNavigator", "Select", 0, SinkKind.XPathInjection),
        new("XPathNavigator", "Evaluate", 0, SinkKind.XPathInjection),
        new("XDocument", "XPathSelectElements", 0, SinkKind.XPathInjection),

        // A03 — XSS
        new("HttpResponse", "Write", 0, SinkKind.Xss),
        new("HtmlHelper", "Raw", 0, SinkKind.Xss),

        // A10 — SSRF
        new("HttpClient", "GetAsync", 0, SinkKind.Ssrf),
        new("HttpClient", "PostAsync", 0, SinkKind.Ssrf),
        new("HttpClient", "SendAsync", 0, SinkKind.Ssrf),
        new("WebClient", "DownloadString", 0, SinkKind.Ssrf),
        new("WebClient", "UploadString", 0, SinkKind.Ssrf),
        new("WebRequest", "Create", 0, SinkKind.Ssrf),

        // A09 — Log injection
        new("ILogger", "LogInformation", 0, SinkKind.LogInjection),
        new("ILogger", "LogWarning", 0, SinkKind.LogInjection),
        new("ILogger", "LogError", 0, SinkKind.LogInjection),
        new("ILogger", "LogDebug", 0, SinkKind.LogInjection),
        new("ILogger", "Log", 1, SinkKind.LogInjection),
    ];

    // SinkIndex declared after AllSinks to ensure correct static initialization order.
    private static readonly Dictionary<(string TypeName, string MemberName), List<SinkDefinition>> SinkIndex =
        BuildIndex();

    private static Dictionary<(string, string), List<SinkDefinition>> BuildIndex()
    {
        var index = new Dictionary<(string, string), List<SinkDefinition>>();
        foreach (var sink in AllSinks)
        {
            var key = (sink.TypeName, sink.MemberName);
            if (!index.TryGetValue(key, out var list))
            {
                list = [];
                index[key] = list;
            }
            list.Add(sink);
        }
        return index;
    }

    /// <summary>
    /// Checks if the given member symbol + argument index constitutes a taint sink.
    /// Returns the SinkKind if it matches, null otherwise.
    /// Lookup is O(1) via pre-built index.
    /// </summary>
    public static SinkKind? GetSinkKind(ISymbol memberSymbol, int argumentIndex = 0)
    {
        var typeName = memberSymbol.ContainingType?.Name ?? string.Empty;
        var memberName = memberSymbol.Name;

        if (!SinkIndex.TryGetValue((typeName, memberName), out var candidates))
            return null;

        foreach (var sink in candidates)
        {
            if (sink.TaintedArgumentIndex == -1 || sink.TaintedArgumentIndex == argumentIndex)
                return sink.Kind;
        }

        return null;
    }
}
