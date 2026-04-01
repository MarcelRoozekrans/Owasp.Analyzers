using Owasp.Analyzers.Analyzers.A01;

namespace Owasp.Analyzers.Tests.A01;

public class HardcodedRoleAnalyzerTests
{
    private readonly HardcodedRoleAnalyzer _analyzer = new();

    [Fact]
    public async Task HardcodedRoleInAuthorize_ShouldDiagnosticA01002()
    {
        var code = """
            [Authorize(Roles = "Admin")]
            public class Test { }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01002");
    }

    [Fact]
    public async Task IsInRoleWithLiteral_ShouldDiagnosticA01003()
    {
        var code = """
            public class Test
            {
                public void M(System.Security.Claims.ClaimsPrincipal user)
                {
                    var isAdmin = user.IsInRole("Admin");
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01003");
    }
}
