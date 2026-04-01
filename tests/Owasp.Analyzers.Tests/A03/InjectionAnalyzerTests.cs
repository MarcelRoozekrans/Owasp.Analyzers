using Owasp.Analyzers.Analyzers.A03;

namespace Owasp.Analyzers.Tests.A03;

public class InjectionAnalyzerTests
{
    private readonly InjectionAnalyzer _analyzer = new();

    [Fact]
    public async Task SqlInjection_StringConcat_ShouldDiagnosticA03001()
    {
        var code = """
            public class SqlCommand
            {
                public string CommandText { get; set; } = "";
            }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var id = Request.Query["id"];
                    var cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM users WHERE id=" + id;
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03001");
    }

    [Fact]
    public async Task SqlInjection_Interpolation_ShouldDiagnosticA03001()
    {
        var code = """
            public class SqlCommand { public string CommandText { get; set; } = ""; }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var id = Request.Query["id"];
                    var cmd = new SqlCommand();
                    cmd.CommandText = $"SELECT * FROM users WHERE id={id}";
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03001");
    }

    [Fact]
    public async Task SqlCommand_WithStaticString_ShouldNotDiagnostic()
    {
        var code = """
            public class SqlCommand { public string CommandText { get; set; } = ""; }
            public class TestController
            {
                public void Action()
                {
                    var cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM users WHERE id=@id";
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA03001");
    }

    [Fact]
    public async Task PathTraversal_ShouldDiagnosticA03003()
    {
        var code = """
            public static class File { public static string ReadAllText(string path) => ""; }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var filename = Request.Query["file"];
                    var content = File.ReadAllText(filename);
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03003");
    }

    [Fact]
    public async Task PathTraversal_WithSanitizer_ShouldNotDiagnostic()
    {
        var code = """
            public static class File { public static string ReadAllText(string path) => ""; }
            public static class Uri { public static string EscapeDataString(string s) => s; }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var filename = Request.Query["file"];
                    var safe = Uri.EscapeDataString(filename);
                    var content = File.ReadAllText(safe);
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA03003");
    }

    [Fact]
    public async Task LdapInjection_ShouldDiagnosticA03004()
    {
        var code = """
            public class DirectorySearcher
            {
                public string Filter { get; set; } = "";
            }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var username = Request.Query["user"];
                    var searcher = new DirectorySearcher();
                    searcher.Filter = "(&(objectClass=user)(sAMAccountName=" + username + "))";
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03004");
    }

    [Fact]
    public async Task XssInjection_ShouldDiagnosticA03006()
    {
        var code = """
            public class HttpResponse
            {
                public void Write(string value) { }
            }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                private HttpResponse Response { get; set; } = null!;
                public void Action()
                {
                    var name = Request.Query["name"];
                    Response.Write(name);
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03006");
    }

    [Fact]
    public async Task XPathInjection_ShouldDiagnosticA03005()
    {
        var code = """
            public class XPathNavigator
            {
                public object Select(string xpath) => null!;
            }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var id = Request.Query["id"];
                    var nav = new XPathNavigator();
                    var result = nav.Select("/users/user[@id='" + id + "']");
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03005");
    }

    [Fact]
    public async Task OsCommandInjection_ShouldDiagnosticA03002()
    {
        var code = """
            public static class Process { public static void Start(string filename, string args = "") { } }
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    var cmd = Request.Query["cmd"];
                    Process.Start(cmd);
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA03002");
    }
}
