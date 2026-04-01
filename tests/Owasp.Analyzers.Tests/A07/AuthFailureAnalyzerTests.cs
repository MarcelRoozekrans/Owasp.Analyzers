using Owasp.Analyzers.Analyzers.A07;

namespace Owasp.Analyzers.Tests.A07;

public class AuthFailureAnalyzerTests
{
    private readonly AuthFailureAnalyzer _analyzer = new();

    [Fact]
    public async Task SecurityAlgorithmsNone_ShouldDiagnosticA07001()
    {
        var code = """
            public class TokenService
            {
                public void CreateToken()
                {
                    var alg = SecurityAlgorithms.None;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07001");
    }

    [Fact]
    public async Task NonNoneAlgorithm_ShouldNotDiagnosticA07001()
    {
        var code = """
            public class TokenService
            {
                public void CreateToken()
                {
                    var alg = SecurityAlgorithms.HmacSha256;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA07001");
    }

    [Fact]
    public async Task ValidateLifetimeFalse_ShouldDiagnosticA07002()
    {
        var code = """
            public class TokenValidator
            {
                public void Validate()
                {
                    var parameters = new TokenValidationParameters
                    {
                        ValidateLifetime = false
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07002");
    }

    [Fact]
    public async Task ValidateLifetimeTrue_ShouldNotDiagnosticA07002()
    {
        var code = """
            public class TokenValidator
            {
                public void Validate()
                {
                    var parameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA07002");
    }

    [Fact]
    public async Task ValidateIssuerSigningKeyFalse_ShouldDiagnosticA07003()
    {
        var code = """
            public class TokenValidator
            {
                public void Validate()
                {
                    var parameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07003");
    }

    [Fact]
    public async Task CookieMissingHttpOnly_ShouldDiagnosticA07004()
    {
        var code = """
            public class CookieService
            {
                public void SetCookie()
                {
                    var options = new CookieOptions
                    {
                        Secure = true
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07004");
    }

    [Fact]
    public async Task CookieMissingSecure_ShouldDiagnosticA07004()
    {
        var code = """
            public class CookieService
            {
                public void SetCookie()
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = true
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07004");
    }

    [Fact]
    public async Task CookieBothFlags_ShouldNotDiagnosticA07004()
    {
        var code = """
            public class CookieService
            {
                public void SetCookie()
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA07004");
    }

    [Fact]
    public async Task SameSiteNoneWithoutSecure_ShouldDiagnosticA07005()
    {
        var code = """
            public class CookieService
            {
                public void SetCookie()
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA07005");
    }

    [Fact]
    public async Task SameSiteNoneWithSecure_ShouldNotDiagnosticA07005()
    {
        var code = """
            public class CookieService
            {
                public void SetCookie()
                {
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA07005");
    }
}
