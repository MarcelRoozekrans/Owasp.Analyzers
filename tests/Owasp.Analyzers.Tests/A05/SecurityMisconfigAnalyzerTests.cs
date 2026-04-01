using Owasp.Analyzers.Analyzers.A05;

namespace Owasp.Analyzers.Tests.A05;

public class SecurityMisconfigAnalyzerTests
{
    private readonly SecurityMisconfigAnalyzer _analyzer = new();

    [Fact]
    public async Task UseDeveloperExceptionPage_Unconditional_ShouldDiagnosticA05001()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseDeveloperExceptionPage();
                    app.UseHttpsRedirection();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA05001");
    }

    [Fact]
    public async Task UseDeveloperExceptionPage_InDevCheck_ShouldNotDiagnostic()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app, object env)
                {
                    if (env.IsDevelopment())
                        app.UseDeveloperExceptionPage();
                    app.UseHttpsRedirection();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA05001");
    }

    [Fact]
    public async Task Configure_WithoutHttpsRedirection_ShouldDiagnosticA05002()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app) { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA05002");
    }

    [Fact]
    public async Task Configure_WithHttpsRedirection_ShouldNotDiagnosticA05002()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseHttpsRedirection();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA05002");
    }

    [Fact]
    public async Task UseDirectoryBrowser_ShouldDiagnosticA05003()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseDirectoryBrowser();
                    app.UseHttpsRedirection();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA05003");
    }

    [Fact]
    public async Task HardcodedPassword_ShouldDiagnosticA05006()
    {
        var code = """
            public class Config
            {
                private string password = "hunter2";
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA05006");
    }

    [Fact]
    public async Task NonCredentialString_ShouldNotDiagnosticA05006()
    {
        var code = """
            public class Config
            {
                private string connectionString = "Server=localhost;Database=mydb";
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA05006");
    }
}
