using System.Text;

using GeometryDash.Server.Serialization.Generator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.XUnit;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public sealed class SerializerGeneratorVerifier : SourceGeneratorVerifier<SerializerGenerator>
{
    public sealed class Test : CSharpSourceGeneratorTest<SerializerGenerator, XUnitVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;

            SolutionTransforms.Add((solution, projectId) => solution.GetProject(projectId)!
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(ServerSerializer).Assembly.Location))
                .Solution);
        }
    }

    private static string FullGeneratedSourceItemName(string testDataFileName)
    {
        var gen = typeof(SerializerGenerator);
        return Path.Combine(gen.Assembly.GetName().Name!, gen.FullName!, testDataFileName);
    }

    public static Test MakeTestFromFileNames(IEnumerable<string> sources, IEnumerable<string> generated)
    {
        var test = new Test();

        test.TestState.Sources.AddRange(sources.Select(s => (
            s,
            SourceText.From(File.ReadAllText(Path.Combine(Constants.SourceFolderPath, s)))
        )));

        test.TestState.GeneratedSources.AddRange(generated.Select(g => (
            FullGeneratedSourceItemName(g),
            SourceText.From(File.ReadAllText(Path.Combine(Constants.GeneratedSourceFolderPath, g)), Encoding.UTF8)
        )));

        return test;
    }

    public static Task Verify(IEnumerable<string> sources, IEnumerable<string> generated)
        => MakeTestFromFileNames(sources, generated).RunAsync();
}
