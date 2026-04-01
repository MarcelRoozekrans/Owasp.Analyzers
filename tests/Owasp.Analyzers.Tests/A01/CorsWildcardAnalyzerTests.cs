using Owasp.Analyzers.Analyzers.A01;

namespace Owasp.Analyzers.Tests.A01;

public class CorsWildcardAnalyzerTests
{
    private readonly CorsWildcardAnalyzer _analyzer = new();

    [Fact]
    public async Task AllowAnyOrigin_ShouldDiagnostic()
    {
        var code = """
            public class Startup
            {
                public void M(object policy)
                {
                    policy.AllowAnyOrigin();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01004");
    }

    [Fact]
    public async Task WithOrigins_ShouldNotDiagnostic()
    {
        var code = """
            public class Startup
            {
                public void M(object policy)
                {
                    policy.WithOrigins("https://example.com");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01004");
    }
}
