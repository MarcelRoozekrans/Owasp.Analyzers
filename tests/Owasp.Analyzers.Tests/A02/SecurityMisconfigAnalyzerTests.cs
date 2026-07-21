using Owasp.Analyzers.Analyzers.A02;

namespace Owasp.Analyzers.Tests.A02;

public class SecurityMisconfigAnalyzerTests
{
    private readonly SecurityMisconfigAnalyzer _analyzer = new();

    [Fact]
    public async Task UseDeveloperExceptionPage_Unconditional_ShouldDiagnosticA02001()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02001");
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02001");
    }

    [Fact]
    public async Task Configure_WithoutHttpsRedirection_ShouldDiagnosticA02002()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app) { }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02002");
    }

    [Fact]
    public async Task Configure_WithHttpsRedirection_ShouldNotDiagnosticA02002()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02002");
    }

    [Fact]
    public async Task UseDirectoryBrowser_ShouldDiagnosticA02003()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02003");
    }

    [Fact]
    public async Task UseDirectoryBrowser_Absent_ShouldNotDiagnosticA02003()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02003");
    }

    [Fact]
    public async Task IncludeErrorDetails_True_ShouldDiagnosticA02004()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseHttpsRedirection();
                    options.IncludeErrorDetails = true;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task IncludeErrorDetails_False_ShouldNotDiagnosticA02004()
    {
        var code = """
            public class Startup
            {
                public void Configure(object app)
                {
                    app.UseHttpsRedirection();
                    options.IncludeErrorDetails = false;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task ConfigureServices_WithoutAddAntiforgery_ShouldDiagnosticA02005()
    {
        var code = """
            public class Startup
            {
                public void ConfigureServices(object services)
                {
                    services.AddMvc();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task ConfigureServices_WithAddAntiforgery_ShouldNotDiagnosticA02005()
    {
        var code = """
            public class Startup
            {
                public void ConfigureServices(object services)
                {
                    services.AddMvc();
                    services.AddAntiforgery();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task HardcodedPassword_ShouldDiagnosticA02006()
    {
        var code = """
            public class Config
            {
                private string password = "hunter2";
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02006");
    }

    [Fact]
    public async Task NonCredentialString_ShouldNotDiagnosticA02006()
    {
        var code = """
            public class Config
            {
                private string connectionString = "Server=localhost;Database=mydb";
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02006");
    }
}
