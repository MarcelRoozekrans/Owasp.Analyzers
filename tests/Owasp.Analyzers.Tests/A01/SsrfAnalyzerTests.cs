using Owasp.Analyzers.Analyzers.A01;

namespace Owasp.Analyzers.Tests.A01;

public class SsrfAnalyzerTests
{
    private readonly SsrfAnalyzer _analyzer = new();

    [Fact]
    public async Task TaintedUrlToHttpClient_ShouldDiagnosticA01006()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01006");
    }

    [Fact]
    public async Task HardcodedUrl_ShouldNotDiagnosticA01006()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01006");
    }

    [Fact]
    public async Task TaintedUrlToWebRequest_ShouldDiagnosticA01007()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01007");
    }

    [Fact]
    public async Task AllowAutoRedirectTrue_ShouldDiagnosticA01008()
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
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01008");
    }

    [Fact]
    public async Task TaintedUrlToPostAsync_ShouldDiagnosticA01006()
    {
        var code = """
            public class Controller
            {
                private HttpRequest Request { get; set; } = null!;
                public void Action()
                {
                    var url = Request.Query["url"];
                    var client = new HttpClient();
                    client.PostAsync(url, null);
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01006");
    }

    [Fact]
    public async Task TaintedUrlToDownloadString_ShouldDiagnosticA01007()
    {
        var code = """
            public class Controller
            {
                private HttpRequest Request { get; set; } = null!;
                public void Action()
                {
                    var url = Request.Query["url"];
                    new WebClient().DownloadString(url);
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01007");
    }

    [Fact]
    public async Task AllowAutoRedirectFalse_ShouldNotDiagnosticA01008()
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
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01008");
    }
}
