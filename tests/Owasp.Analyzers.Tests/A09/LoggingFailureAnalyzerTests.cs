using Owasp.Analyzers.Analyzers.A09;

namespace Owasp.Analyzers.Tests.A09;

public class LoggingFailureAnalyzerTests
{
    private readonly LoggingFailureAnalyzer _analyzer = new();

    [Fact]
    public async Task EmptyCatch_ShouldDiagnosticA09001()
    {
        var code = """
            public class Example
            {
                public void Method()
                {
                    try { int x = 1; }
                    catch { }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09001");
    }

    [Fact]
    public async Task EmptyCatch_WithComment_ShouldDiagnosticA09001()
    {
        var code = """
            public class Example
            {
                public void Method()
                {
                    try { int x = 1; }
                    catch
                    {
                        // intentionally empty
                    }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09001");
    }

    [Fact]
    public async Task NonEmptyCatch_ShouldNotDiagnosticA09001()
    {
        var code = """
            public class Example
            {
                public void Method()
                {
                    try { int x = 1; }
                    catch
                    {
                        int y = 2;
                    }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA09001");
    }

    [Fact]
    public async Task CatchWithoutLogging_ShouldDiagnosticA09002()
    {
        var code = """
            public class Example
            {
                public void Method()
                {
                    try { int x = 1; }
                    catch
                    {
                        int y = 2;
                    }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task CatchWithLogging_ShouldNotDiagnosticA09002()
    {
        var code = """
            using System;
            public class Example
            {
                public void Method()
                {
                    try { int x = 1; }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task CatchWithILogger_ShouldNotDiagnosticA09002()
    {
        var code = """
            using System;
            using Microsoft.Extensions.Logging;
            public class Example
            {
                private readonly ILogger _logger;
                public Example(ILogger logger) { _logger = logger; }
                public void Method()
                {
                    try { int x = 1; }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "error");
                    }
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA09002");
    }

    [Fact]
    public async Task SensitiveKeywordInLog_ShouldDiagnosticA09004()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09004");
    }

    [Fact]
    public async Task SensitiveKeywordInLog_Token_ShouldDiagnosticA09004()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA09004");
    }

    [Fact]
    public async Task NonSensitiveLog_ShouldNotDiagnosticA09004()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA09004");
    }
}
