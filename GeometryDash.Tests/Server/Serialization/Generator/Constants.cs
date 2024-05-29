using System.Reflection;

namespace GeometryDash.Tests.Server.Serialization.Generator;
public static class Constants
{
    public static string SourceFolderFullPath { get; }
    public static string GeneratedSourceFolderPath { get; }
    public static string GeneratedSourceFolderFullPath { get; }
    public static string TestDataPath { get; }

    static Constants()
    {
        TestDataPath = Path.Combine("Server", "Serialization", "Generator", "TestData");
        GeneratedSourceFolderPath = Path.Combine(TestDataPath, "GeneratedSources");

        var executingAsmPath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);

        GeneratedSourceFolderFullPath = Path.Combine(executingAsmPath, GeneratedSourceFolderPath);

        var testDataFullPath = Path.Combine(executingAsmPath, TestDataPath);
        SourceFolderFullPath = Path.Combine(testDataFullPath, "Sources");
    }
}
