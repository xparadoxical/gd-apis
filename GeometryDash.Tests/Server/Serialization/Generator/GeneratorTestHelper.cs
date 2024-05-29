using System.Runtime.CompilerServices;

using GeometryDash.Server.Serialization.Generator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public static class GeneratorTestHelper
{
    public static GeneratorDriver CreateDriverForSource(string source)
    {
        var compilation = CSharpCompilation.Create(
            "Tests",
            [CSharpSyntaxTree.ParseText(source)],
            [
                .. Basic.Reference.Assemblies.Net80.References.All,
                MetadataReference.CreateFromFile(typeof(ServerSerializer).Assembly.Location)
            ]
        );

        return CSharpGeneratorDriver.Create(new SerializerGenerator())
            .RunGenerators(compilation);
    }

    public static GeneratorDriver CreateDriverFromFile(string fileName)
    {
        var source = File.ReadAllText(Path.Combine(Constants.SourceFolderFullPath, fileName));
        return CreateDriverForSource(source);
    }

    public static GeneratorDriver CreateDriverForTest(
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "")
    {
        var callingType = Path.GetFileNameWithoutExtension(callerFilePath);
        return CreateDriverFromFile($"{callingType}.{callerMemberName}.cs");
    }
}
