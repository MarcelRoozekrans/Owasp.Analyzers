using Owasp.Analyzers.Analyzers.A01;

namespace Owasp.Analyzers.Tests.A01;

public class MissingAntiforgeryTokenAnalyzerTests
{
    private readonly MissingAntiforgeryTokenAnalyzer _analyzer = new();

    [Fact]
    public async Task HttpPostWithoutAntiforgery_ShouldDiagnostic()
    {
        var code = """
            public class Test
            {
                [HttpPost]
                public void Submit() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01005");
    }

    [Fact]
    public async Task HttpPostWithAntiforgery_ShouldNotDiagnostic()
    {
        var code = """
            public class Test
            {
                [HttpPost]
                [ValidateAntiForgeryToken]
                public void Submit() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01005");
    }

    [Fact]
    public async Task HttpGet_ShouldNotDiagnostic()
    {
        var code = """
            public class Test
            {
                [HttpGet]
                public void Get() { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01005");
    }
}
