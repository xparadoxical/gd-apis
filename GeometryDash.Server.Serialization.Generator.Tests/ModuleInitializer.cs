using System.Runtime.CompilerServices;

namespace GeometryDash.Tests.Server.Serialization.Generator;
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
