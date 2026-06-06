using System.Runtime.CompilerServices;

namespace GeometryDash.Server.Serialization.Generator.Tests;
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.UseUtf8NoBom();
        VerifierSettings.AutoVerify(false);
        UseProjectRelativeDirectory(Constants.GeneratedSourceFolderPath);

        VerifySourceGenerators.Initialize();
    }
}
