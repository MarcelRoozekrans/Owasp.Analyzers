using Owasp.Analyzers.Analyzers.A02;

namespace Owasp.Analyzers.Tests.A02;

public class WeakCryptoAnalyzerTests
{
    private readonly WeakCryptoAnalyzer _analyzer = new();

    [Fact]
    public async Task MD5Create_ShouldDiagnosticA02001()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var md5 = MD5.Create();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02001");
    }

    [Fact]
    public async Task SHA1Create_ShouldDiagnosticA02001()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var sha = SHA1.Create();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02001");
    }

    [Fact]
    public async Task AesCreate_ShouldNotDiagnostic()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var aes = Aes.Create();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02001");
    }

    [Fact]
    public async Task EcbMode_ShouldDiagnosticA02002()
    {
        var code = """
            public class C
            {
                public void M(object aes)
                {
                    aes.Mode = CipherMode.ECB;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02002");
    }

    [Fact]
    public async Task CbcMode_ShouldNotDiagnostic()
    {
        var code = """
            public class C
            {
                public void M(object aes)
                {
                    aes.Mode = CipherMode.CBC;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02002");
    }

    [Fact]
    public async Task NewRandom_ShouldDiagnosticA02003()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var rng = new Random();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02003");
    }

    [Fact]
    public async Task NewMD5CryptoServiceProvider_ShouldDiagnosticA02001()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var md5 = new MD5CryptoServiceProvider();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02001");
    }

    [Fact]
    public async Task NewSHA1Managed_ShouldDiagnosticA02001()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var sha = new SHA1Managed();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02001");
    }
}
