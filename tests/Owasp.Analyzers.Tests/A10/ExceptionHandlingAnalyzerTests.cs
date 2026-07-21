using Owasp.Analyzers.Analyzers.A10;

namespace Owasp.Analyzers.Tests.A10;

public class ExceptionHandlingAnalyzerTests
{
    private readonly ExceptionHandlingAnalyzer _analyzer = new();

    [Fact]
    public async Task EmptyCatch_ShouldDiagnosticA10001()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10001");
    }

    [Fact]
    public async Task EmptyCatch_WithComment_ShouldDiagnosticA10001()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10001");
    }

    [Fact]
    public async Task NonEmptyCatch_ShouldNotDiagnosticA10001()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA10001");
    }

    [Fact]
    public async Task CatchWithoutLogging_ShouldDiagnosticA10002()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10002");
    }

    [Fact]
    public async Task CatchWithLogging_ShouldNotDiagnosticA10002()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA10002");
    }

    [Fact]
    public async Task CatchWithILogger_ShouldNotDiagnosticA10002()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA10002");
    }
}
