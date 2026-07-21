using Owasp.Analyzers.Analyzers.A09;

namespace Owasp.Analyzers.Tests.A09;

public class LoggingFailureAnalyzerTests
{
    private readonly LoggingFailureAnalyzer _analyzer = new();

    [Fact]
    public async Task SensitiveKeywordInLog_ShouldDiagnosticA09002()
    {
        var code = """
            using System;
            public class Example
            {
                public void Method()
                {
                    Console.WriteLine("User password: secret123");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task SensitiveKeywordInLog_Token_ShouldDiagnosticA09002()
    {
        var code = """
            using System;
            public class Example
            {
                public void Method()
                {
                    Console.WriteLine("auth token: abc");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task NonSensitiveLog_ShouldNotDiagnosticA09002()
    {
        var code = """
            using System;
            public class Example
            {
                public void Method()
                {
                    Console.WriteLine("User logged in successfully");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task TaintedInputToLog_ShouldDiagnosticA09001()
    {
        var code = """
            public class Controller
            {
                private HttpRequest Request { get; set; } = null!;
                public void Action()
                {
                    var userInput = Request.Query["search"];
                    System.Console.WriteLine(userInput);
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        AnalyzerTestHelper.AssertSingleDiagnostic(diagnostics, "OWASPA09001");
    }
}
