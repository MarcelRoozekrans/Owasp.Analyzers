using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Owasp.Analyzers.Taint;

namespace Owasp.Analyzers.Tests.Taint;

public class TaintEngineTests
{
    [Fact]
    public void DirectAssignment_PropagatesTaint()
    {
        var code = """
            var query = Request.Query["id"];
            var x = query;
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.Contains("x", taintedLocals);
    }

    [Fact]
    public void StringInterpolation_PropagatesTaint()
    {
        var code = """
            var id = Request.Query["id"];
            var sql = $"SELECT * FROM users WHERE id = {id}";
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.Contains("sql", taintedLocals);
    }

    [Fact]
    public void StringConcatenation_PropagatesTaint()
    {
        var code = """
            var id = Request.Query["id"];
            var sql = "SELECT * FROM users WHERE id = " + id;
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.Contains("sql", taintedLocals);
    }

    [Fact]
    public void HtmlEncode_ClearsTaint()
    {
        var code = """
            var input = Request.Query["name"];
            var safe = System.Web.HttpUtility.HtmlEncode(input);
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.DoesNotContain("safe", taintedLocals);
    }

    [Fact]
    public void UntaintedVariable_NotTainted()
    {
        var code = """
            var id = "hardcoded";
            var sql = "SELECT * FROM users WHERE id = " + id;
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.DoesNotContain("id", taintedLocals);
        Assert.DoesNotContain("sql", taintedLocals);
    }

    [Fact]
    public void ReAssignment_PropagatesTaint()
    {
        var code = """
            string query;
            query = Request.Query["id"];
            var sql = "SELECT * FROM users WHERE id = " + query;
            """;
        var (taintedLocals, _) = RunTaintEngine(code);
        Assert.Contains("query", taintedLocals);
        Assert.Contains("sql", taintedLocals);
    }

    [Fact]
    public void TaintDoesNotLeakAcrossMethods()
    {
        var code = """
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void MethodA()
                {
                    var id = Request.Query["id"];
                }
                public void MethodB()
                {
                    var id = "hardcoded";
                    var sql = "SELECT * FROM users WHERE id=" + id;
                }
            }
            public class TestRequest { public TestQuery Query { get; set; } = null!; }
            public class TestQuery { public string this[string key] => key; }
            """;
        var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);
        var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Test")
            .AddReferences(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(tree);
        var model = compilation.GetSemanticModel(tree);
        var engine = new Owasp.Analyzers.Taint.TaintEngine(model);
        engine.Analyze(tree.GetRoot());
        Assert.Empty(engine.SinkHits); // id in MethodB is not tainted
        Assert.DoesNotContain("sql", engine.TaintedLocals); // sql in MethodB is not tainted
    }

    private static (HashSet<string> taintedLocals, List<TaintSinks.SinkKind> sinkHits) RunTaintEngine(string snippet)
    {
        var code = $$"""
            public class TestController
            {
                private TestRequest Request { get; set; } = null!;
                public void Action()
                {
                    {{snippet}}
                }
            }
            public class TestRequest
            {
                public TestQuery Query { get; set; } = null!;
            }
            public class TestQuery
            {
                public string this[string key] => key;
            }
            """;
        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("Test")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(tree);
        var model = compilation.GetSemanticModel(tree);

        var engine = new TaintEngine(model);
        engine.Analyze(tree.GetRoot());
        return (engine.TaintedLocals, engine.SinkHits);
    }
}
