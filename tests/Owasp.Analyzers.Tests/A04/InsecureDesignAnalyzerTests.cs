using Owasp.Analyzers.Analyzers.A04;

namespace Owasp.Analyzers.Tests.A04;

public class InsecureDesignAnalyzerTests
{
    private readonly InsecureDesignAnalyzer _analyzer = new();

    [Fact]
    public async Task LoginEndpoint_WithoutRateLimit_ShouldDiagnosticA04002()
    {
        var code = """
            public class AuthController
            {
                [HttpPost]
                public void Login() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA04002");
    }

    [Fact]
    public async Task LoginEndpoint_WithRateLimit_ShouldNotDiagnostic()
    {
        var code = """
            public class AuthController
            {
                [HttpPost]
                [EnableRateLimiting("fixed")]
                public void Login() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA04002");
    }

    [Fact]
    public async Task LoginEndpoint_WithHttpDelete_ShouldNotDiagnosticA04002()
    {
        var code = """
            public class AuthController
            {
                [HttpDelete]
                public void Login() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA04002");
    }

    [Fact]
    public async Task NonAuthEndpoint_ShouldNotDiagnostic()
    {
        var code = """
            public class OrdersController
            {
                [HttpPost]
                public void CreateOrder() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA04002");
    }
}
