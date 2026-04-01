using Owasp.Analyzers.Analyzers.A02;

namespace Owasp.Analyzers.Tests.A02;

public class InsecureTlsAnalyzerTests
{
    private readonly InsecureTlsAnalyzer _analyzer = new();

    [Fact]
    public async Task HttpUrl_ShouldDiagnosticA02007()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var url = "http://example.com";
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02007");
    }

    [Fact]
    public async Task HttpsUrl_ShouldNotDiagnostic()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var url = "https://example.com";
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02007");
    }

    [Fact]
    public async Task CertValidationBypass_ShouldDiagnosticA02006()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02006");
    }

    [Fact]
    public async Task LegacyTls_ShouldDiagnosticA02005()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task Ssl3Protocol_ShouldDiagnosticA02005()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task Tls12Protocol_ShouldNotDiagnosticA02005()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task CombinedTlsFlags_ShouldDiagnosticA02005()
    {
        var code = """
            public class Test
            {
                public void M()
                {
                    System.Net.ServicePointManager.SecurityProtocol =
                        System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02005");
    }

    [Fact]
    public async Task HttpSchemeOnly_ShouldNotDiagnostic()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var scheme = "http://";
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02007");
    }
}
