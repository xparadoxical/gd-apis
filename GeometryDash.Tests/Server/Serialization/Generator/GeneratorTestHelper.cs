using System.Runtime.CompilerServices;

using GeometryDash.Server.Serialization.Generator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public static class GeneratorTestHelper
{
    public sealed record GeneratorData(GeneratorDriver Driver, Compilation Input, Compilation Output);

    public static GeneratorData CreateDriverForSource(string source, string path = "")
    {
        var inputCompilation = CSharpCompilation.Create(
            "Tests",
            [CSharpSyntaxTree.ParseText(source, path: path)],
            [
                .. Basic.Reference.Assemblies.Net80.References.All,
                MetadataReference.CreateFromFile(typeof(ServerSerializer).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(CommunityToolkit.HighPerformance.ArrayExtensions).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true)
        );

        var driver = CSharpGeneratorDriver.Create([new SerializerGenerator().AsSourceGenerator()],
            parseOptions: new CSharpParseOptions())
            .RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var generatorDiagnostics /*most likely*/);
        Assert.Empty(generatorDiagnostics);
        return new(driver, inputCompilation, outputCompilation);
    }

    public static GeneratorData CreateDriverFromFile(string fileName)
    {
        var source = File.ReadAllText(Path.Combine(Constants.SourceFolderFullPath, fileName));
        return CreateDriverForSource(source, fileName);
    }

    public static GeneratorData CreateDriverForTest(
        [CallerFilePath] string callerFilePath = "",
        [CallerMemberName] string callerMemberName = "")
    {
        var callingType = Path.GetFileNameWithoutExtension(callerFilePath);
        return CreateDriverFromFile($"{callingType}.{callerMemberName}.cs");
    }

    public static async Task Verify(GeneratorDriverRunResult result, GeneratorData sgData, [CallerFilePath] string sourceFile = "")
    {
        Assert.NotEmpty(result.GeneratedTrees);
        await Verifier.Verify(result, sourceFile: sourceFile);
        await Verifier.Verify(sgData.Input.GetDiagnostics(), sourceFile: sourceFile)
            .UseTextForParameters("inputdiag");
        await Verifier.Verify(sgData.Output.GetDiagnostics(), sourceFile: sourceFile)
            .UseTextForParameters("outputdiag");
    }
}
