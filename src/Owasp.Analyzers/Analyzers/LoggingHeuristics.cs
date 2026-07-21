using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers;

/// <summary>
/// Shared "is this a logging call?" heuristic used by the A09 (Security Logging and Alerting
/// Failures) and A10 (Mishandling of Exceptional Conditions) analyzers, both of which reason
/// about logging calls inside catch blocks and invocation expressions.
/// </summary>
internal static class LoggingHeuristics
{
    public static readonly ImmutableHashSet<string> LoggingMethodNames = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase,
        "Log", "LogInformation", "LogWarning", "LogError", "LogDebug", "LogTrace", "LogCritical",
        "WriteLine", "Write", "Info", "Error", "Warn", "Debug", "Fatal");

    public static string? GetInvokedMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax member => member.Name.Identifier.Text,
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            _ => null
        };
    }
}
