using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Owasp.Analyzers.Analyzers.A08;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsafeDeserializationAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule001 = new("OWASPA08001",
        "BinaryFormatter is unsafe for deserialization",
        "BinaryFormatter is insecure and can lead to remote code execution — use a safe alternative such as System.Text.Json or XmlSerializer",
        "OWASP.A08", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule002 = new("OWASPA08002",
        "Unsafe deserializer usage",
        "'{0}' is an insecure deserializer and can lead to remote code execution — use a safe alternative",
        "OWASP.A08", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule003 = new("OWASPA08003",
        "Unsafe TypeNameHandling setting in Json.NET",
        "TypeNameHandling values other than None allow type injection attacks — use TypeNameHandling.None",
        "OWASP.A08", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor Rule004 = new("OWASPA08004",
        "JavaScriptSerializer with SimpleTypeResolver is unsafe",
        "JavaScriptSerializer with SimpleTypeResolver can lead to remote code execution — avoid SimpleTypeResolver or use a safe serializer",
        "OWASP.A08", DiagnosticSeverity.Error, isEnabledByDefault: true);

    private static readonly ImmutableArray<string> DangerousDeserializers =
        ["NetDataContractSerializer", "SoapFormatter", "LosFormatter", "ObjectStateFormatter"];

    private static readonly ImmutableArray<string> DangerousTypeNameHandlingValues =
        ["All", "Auto", "Objects", "Arrays"];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Rule001, Rule002, Rule003, Rule004];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;
        var typeName = creation.Type.ToString();

        // OWASPA08001 — BinaryFormatter
        if (typeName.EndsWith("BinaryFormatter", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule001, creation.GetLocation()));
            return;
        }

        // OWASPA08002 — other dangerous deserializers
        foreach (var dangerous in DangerousDeserializers)
        {
            if (typeName.EndsWith(dangerous, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule002, creation.GetLocation(), dangerous));
                return;
            }
        }

        // OWASPA08003 — JsonSerializerSettings with dangerous TypeNameHandling in initializer
        if (typeName.EndsWith("JsonSerializerSettings", StringComparison.Ordinal) &&
            creation.Initializer != null)
        {
            foreach (var expression in creation.Initializer.Expressions)
            {
                if (expression is AssignmentExpressionSyntax assignment &&
                    assignment.Left is IdentifierNameSyntax propertyName &&
                    propertyName.Identifier.Text == "TypeNameHandling")
                {
                    var value = assignment.Right.ToString();
                    var rhsToken = value.Contains('.') ? value[(value.LastIndexOf('.') + 1)..] : value;
                    foreach (var dangerous in DangerousTypeNameHandlingValues)
                    {
                        if (rhsToken.Equals(dangerous, StringComparison.Ordinal))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule003, assignment.GetLocation()));
                            break;
                        }
                    }
                }
            }
        }

        // OWASPA08004 — JavaScriptSerializer with SimpleTypeResolver argument
        if (typeName.EndsWith("JavaScriptSerializer", StringComparison.Ordinal) &&
            creation.ArgumentList != null)
        {
            foreach (var argument in creation.ArgumentList.Arguments)
            {
                var argText = argument.ToString();
                if (argText.Contains("SimpleTypeResolver"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule004, creation.GetLocation()));
                    break;
                }
            }
        }
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        // OWASPA08003 — standalone assignment to any LHS ending with "TypeNameHandling"
        var lhs = assignment.Left.ToString();
        if (!lhs.EndsWith("TypeNameHandling", StringComparison.Ordinal)) return;

        var rhs = assignment.Right.ToString();
        var rhsLastToken = rhs.Contains('.') ? rhs[(rhs.LastIndexOf('.') + 1)..] : rhs;
        foreach (var dangerous in DangerousTypeNameHandlingValues)
        {
            if (rhsLastToken.Equals(dangerous, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule003, assignment.GetLocation()));
                return;
            }
        }
    }
}
