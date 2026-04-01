using Owasp.Analyzers.Analyzers.A02;

namespace Owasp.Analyzers.Tests.A02;

public class HardcodedKeyAnalyzerTests
{
    private readonly HardcodedKeyAnalyzer _analyzer = new();

    [Fact]
    public async Task HardcodedKeyBytes_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var key = new byte[] { 0x01, 0x02 };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task HardcodedIvBytes_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var iv = new byte[] { 0x01, 0x02 };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task NonKeyByteArray_ShouldNotDiagnostic()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var data = new byte[] { 0x01 };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task HardcodedSecretBytes_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var secret = new byte[] { 0xAA, 0xBB, 0xCC };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task HardcodedKeyField_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                private static byte[] encryptionKey = new byte[] { 0x01, 0x02, 0x03 };
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task HardcodedSaltBytes_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    var salt = new byte[] { 0x11, 0x22, 0x33 };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }

    [Fact]
    public async Task ImplicitByteArrayKey_ShouldDiagnosticA02004()
    {
        var code = """
            public class C
            {
                public void M()
                {
                    byte[] key = new[] { (byte)0x01, (byte)0x02 };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA02004");
    }
}
