using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Owasp.Analyzers.Taint;

/// <summary>
/// Identifies ASP.NET Core sources of user-controlled (tainted) data.
/// </summary>
internal static class TaintSources
{
    // HttpContext.Request property names that yield tainted data
    private static readonly HashSet<string> RequestProperties =
    [
        "Query", "Form", "Headers", "Body", "RouteValues",
        "Cookies", "Path", "QueryString", "ContentType"
    ];

    // Attributes that mark action parameters as coming from user input
    private static readonly HashSet<string> BindingAttributes =
    [
        "FromQuery", "FromBody", "FromRoute", "FromForm",
        "FromQueryAttribute", "FromBodyAttribute", "FromRouteAttribute", "FromFormAttribute"
    ];

    /// <summary>
    /// Returns true if the given parameter symbol represents user-controlled input
    /// (ASP.NET model binding or explicit binding attribute).
    /// </summary>
    public static bool IsUserControlledParameter(IParameterSymbol parameter)
    {
        // Parameters with binding attributes are explicitly tainted
        foreach (var attr in parameter.GetAttributes())
        {
            var name = attr.AttributeClass?.Name ?? string.Empty;
            if (BindingAttributes.Contains(name))
                return true;
        }

        // Parameters of controller action methods are implicitly tainted (model binding)
        if (parameter.ContainingSymbol is IMethodSymbol method &&
            IsControllerAction(method))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the member access is reading from HttpContext.Request.*
    /// (e.g., Request.Query["id"], Request.Form["name"]).
    /// </summary>
    public static bool IsRequestAccess(MemberAccessExpressionSyntax memberAccess)
    {
        var name = memberAccess.Name.Identifier.Text;
        if (!RequestProperties.Contains(name))
            return false;

        // Syntax-only check: receiver must be "Request" or end with ".Request"
        // Full semantic type resolution (confirming HttpRequest type) is deferred to v2.
        var receiver = memberAccess.Expression.ToString();
        return receiver == "Request" ||
               receiver.EndsWith(".Request", StringComparison.Ordinal);
    }

    private static bool IsControllerAction(IMethodSymbol method)
    {
        var containingType = method.ContainingType;
        if (containingType == null) return false;

        // Type inherits from ControllerBase or has [ApiController]/[Controller] attribute
        return InheritsFromControllerBase(containingType) ||
               HasControllerAttribute(containingType);
    }

    private static bool InheritsFromControllerBase(INamedTypeSymbol type)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (current.Name is "Controller" or "ControllerBase")
                return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool HasControllerAttribute(INamedTypeSymbol type)
    {
        return type.GetAttributes().Any(a =>
            a.AttributeClass?.Name is "ApiControllerAttribute" or "ControllerAttribute");
    }
}
