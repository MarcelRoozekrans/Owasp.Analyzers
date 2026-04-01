using Owasp.Analyzers.Analyzers.A10;

namespace Owasp.Analyzers.Tests.A10;

public class SsrfAnalyzerTests
{
    private readonly SsrfAnalyzer _analyzer = new();

    [Fact]
    public async Task TaintedUrlToHttpClient_ShouldDiagnosticA10001()
    {
        var code = """
            public class Controller
            {
                private HttpRequest Request { get; set; } = null!;
                public void Action()
                {
                    var url = Request.Query["url"];
                    var client = new HttpClient();
                    client.GetAsync(url);
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10001");
    }

    [Fact]
    public async Task HardcodedUrl_ShouldNotDiagnosticA10001()
    {
        var code = """
            public class Controller
            {
                public void Action()
                {
                    var client = new HttpClient();
                    client.GetAsync("https://api.example.com");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA10001");
    }

    [Fact]
    public async Task TaintedUrlToWebRequest_ShouldDiagnosticA10002()
    {
        var code = """
            public class Controller
            {
                private HttpRequest Request { get; set; } = null!;
                public void Action()
                {
                    var url = Request.Query["url"];
                    var req = WebRequest.Create(url);
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10002");
    }

    [Fact]
    public async Task AllowAutoRedirectTrue_ShouldDiagnosticA10003()
    {
        var code = """
            public class Controller
            {
                public void Configure()
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = true;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA10003");
    }

    [Fact]
    public async Task AllowAutoRedirectFalse_ShouldNotDiagnosticA10003()
    {
        var code = """
            public class Controller
            {
                public void Configure()
                {
                    var handler = new HttpClientHandler();
                    handler.AllowAutoRedirect = false;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA10003");
    }
}
