using Owasp.Analyzers.Analyzers.A02;

namespace Owasp.Analyzers.Tests.A02;

public class MissingHstsAnalyzerTests
{
    private readonly MissingHstsAnalyzer _analyzer = new();

    [Fact]
    public async Task Configure_WithoutUseHsts_ShouldDiagnosticA02008()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseRouting();
                    app.UseAuthentication();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02008");
    }

    [Fact]
    public async Task Configure_WithUseHsts_ShouldNotDiagnostic()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseHsts();
                    app.UseRouting();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02008");
    }

    [Fact]
    public async Task ConfigureApp_WithoutUseHsts_ShouldDiagnosticA02008()
    {
        var code = """
            public class Program
            {
                public void ConfigureApp(object app)
                {
                    app.UseRouting();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02008");
    }

    [Fact]
    public async Task ConfigureApp_WithUseHsts_ShouldNotDiagnostic()
    {
        var code = """
            public class Program
            {
                public void ConfigureApp(object app)
                {
                    app.UseHsts();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02008");
    }

    [Fact]
    public async Task OtherMethodName_ShouldNotDiagnostic()
    {
        var code = """
            public class Startup
            {
                public void Setup(object app)
                {
                    app.UseRouting();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02008");
    }
}
