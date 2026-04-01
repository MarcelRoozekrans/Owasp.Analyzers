using Owasp.Analyzers.Analyzers.A01;

namespace Owasp.Analyzers.Tests.A01;

public class MissingAuthorizeAnalyzerTests
{
    private readonly MissingAuthorizeAnalyzer _analyzer = new();

    [Fact]
    public async Task ControllerAction_WithoutAuthorize_ShouldDiagnostic()
    {
        var code = """
            [ApiController]
            public class OrdersController : ControllerBase
            {
                [HttpGet]
                public object GetOrders() => null!;
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA01001");
    }

    [Fact]
    public async Task ControllerAction_WithAuthorize_ShouldNotDiagnostic()
    {
        var code = """
            [ApiController]
            public class OrdersController : ControllerBase
            {
                [Authorize]
                [HttpGet]
                public object GetOrders() => null!;
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01001");
    }

    [Fact]
    public async Task ControllerAction_WithAllowAnonymous_ShouldNotDiagnostic()
    {
        var code = """
            [ApiController]
            public class AuthController : ControllerBase
            {
                [AllowAnonymous]
                [HttpPost]
                public object Login() => null!;
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01001");
    }

    [Fact]
    public async Task ClassLevelAuthorize_ShouldNotDiagnostic()
    {
        var code = """
            [Authorize]
            [ApiController]
            public class OrdersController : ControllerBase
            {
                [HttpGet]
                public object GetOrders() => null!;
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA01001");
    }
}
