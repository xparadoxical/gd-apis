using System.Reflection;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public static class Constants
{
    public static string SourceFolderPath { get; }
    public static string GeneratedSourceFolderPath { get; }

    static Constants()
    {
        var executingAsmPath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);

        var testDataPath = Path.Combine(executingAsmPath, "Server", "Serialization", "Generator", "TestData");
        SourceFolderPath = Path.Combine(testDataPath, "Sources");
        GeneratedSourceFolderPath = Path.Combine(testDataPath, "GeneratedSources");
    }
}
