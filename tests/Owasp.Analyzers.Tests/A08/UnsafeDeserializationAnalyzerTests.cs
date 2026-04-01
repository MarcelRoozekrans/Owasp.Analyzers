using Owasp.Analyzers.Analyzers.A08;

namespace Owasp.Analyzers.Tests.A08;

public class UnsafeDeserializationAnalyzerTests
{
    private readonly UnsafeDeserializationAnalyzer _analyzer = new();

    [Fact]
    public async Task BinaryFormatter_ShouldDiagnosticA08001()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var formatter = new BinaryFormatter();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08001");
    }

    [Fact]
    public async Task BinaryFormatter_FullyQualified_ShouldDiagnosticA08001()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08001");
    }

    [Fact]
    public async Task SafeSerializer_ShouldNotDiagnosticA08001()
    {
        var code = """
            public class Serializer
            {
                public void Serialize()
                {
                    var serializer = new XmlSerializer(typeof(string));
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA08001");
    }

    [Fact]
    public async Task NetDataContractSerializer_ShouldDiagnosticA08002()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var serializer = new NetDataContractSerializer();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08002");
    }

    [Fact]
    public async Task SoapFormatter_ShouldDiagnosticA08002()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var formatter = new SoapFormatter();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08002");
    }

    [Fact]
    public async Task LosFormatter_ShouldDiagnosticA08002()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var formatter = new LosFormatter();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08002");
    }

    [Fact]
    public async Task ObjectStateFormatter_ShouldDiagnosticA08002()
    {
        var code = """
            public class Deserializer
            {
                public void Deserialize()
                {
                    var formatter = new ObjectStateFormatter();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08002");
    }

    [Fact]
    public async Task TypeNameHandlingAll_InInitializer_ShouldDiagnosticA08003()
    {
        var code = """
            public class JsonConfig
            {
                public void Configure()
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08003");
    }

    [Fact]
    public async Task TypeNameHandlingAuto_Assignment_ShouldDiagnosticA08003()
    {
        var code = """
            public class JsonConfig
            {
                public void Configure()
                {
                    var settings = new JsonSerializerSettings();
                    settings.TypeNameHandling = TypeNameHandling.Auto;
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08003");
    }

    [Fact]
    public async Task TypeNameHandlingNone_ShouldNotDiagnosticA08003()
    {
        var code = """
            public class JsonConfig
            {
                public void Configure()
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.None
                    };
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA08003");
    }

    [Fact]
    public async Task JavaScriptSerializer_WithSimpleTypeResolver_ShouldDiagnosticA08004()
    {
        var code = """
            public class JsonConfig
            {
                public void Configure()
                {
                    var serializer = new JavaScriptSerializer(new SimpleTypeResolver());
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.Contains(diagnostics, d => d.Id == "OWASPA08004");
    }

    [Fact]
    public async Task JavaScriptSerializer_NoResolver_ShouldNotDiagnosticA08004()
    {
        var code = """
            public class JsonConfig
            {
                public void Configure()
                {
                    var serializer = new JavaScriptSerializer();
                }
            }
            """;
        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(code, _analyzer);
        Assert.DoesNotContain(diagnostics, d => d.Id == "OWASPA08004");
    }
}
