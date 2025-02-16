using System.Runtime.CompilerServices;

using GeometryDash.Server.Serialization.Generator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public static class GeneratorTestHelper
{
    public sealed record GeneratorData(GeneratorDriver Driver, Compilation Input, Compilation Output);

    public static GeneratorData CreateDriverForSource(string source)
    {
        var inputCompilation = CSharpCompilation.Create(
            "Tests",
            [CSharpSyntaxTree.ParseText(source)],
            [
                .. Basic.Reference.Assemblies.Net80.References.All,
                MetadataReference.CreateFromFile(typeof(ServerSerializer).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
        );

        var driver = CSharpGeneratorDriver.Create([new SerializerGenerator().AsSourceGenerator()],
            parseOptions: new CSharpParseOptions())
            .RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out _);
        return new(driver, inputCompilation, outputCompilation);
    }

    public static GeneratorData CreateDriverFromFile(string fileName)
    {
        var source = File.ReadAllText(Path.Combine(Constants.SourceFolderFullPath, fileName));
        return CreateDriverForSource(source);
    }

    public static GeneratorData CreateDriverForTest(
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "")
    {
        var callingType = Path.GetFileNameWithoutExtension(callerFilePath);
        return CreateDriverFromFile($"{callingType}.{callerMemberName}.cs");
    }
}
