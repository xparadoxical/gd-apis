using System.Text;

using ComputeSharp.SourceGeneration.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GeometryDash.Server.Serialization.Generator;
public sealed partial class SerializerGenerator
{
    public static void GenerateType(SourceProductionContext ctx, SerializableClassInfo info)
    {
        var c = info.Class;

        using var writer = new IndentedTextWriter();
        writer.WriteLine(
            $$"""
            using GeometryDash.Server.Serialization;
            using CommunityToolkit.HighPerformance;

            namespace {{c.Namespace}};

            partial {{c.Declarator}} : ISerializable<{{c.Name}}>
            """, true);

        using (writer.WriteBlock())
        {
            //main method
            writer.WriteLine($"public static {c.Name} Deserialize(ReadOnlySpan<byte> input)"); //TODO extract identifier names into consts?
            using (writer.WriteBlock())
            {
                if (c.Keyed)
                    WriteKeyedBody(writer, info);
                else
                    WriteKeylessBody(writer, info);
            }
        }

        ctx.AddSource($"{c.Namespace}.{c.Name}.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
    }

    public static string? NullConditionalAdd(string? left, params string?[] right)
    {
        string? result = left;
        for (int i = 0; result is not null && i < right.Length; i++)
            result += right[i];

        return result;
    }

    public static void WriteKeyedBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine($"var ret = new {info.Class.Name}();"); //TODO recheck if possible to support type args

        writer.WriteLine($"foreach (var (k, v) in new RobTopStringReader(input) {{ FieldSeparator = (byte)'{info.Class.FieldSeparator}' }})");
        using (writer.WriteBlock())
        {
            WriteThrowingLogic(writer, info);
        }

        writer.WriteLine("return ret;");
    }

    public static void WriteKeylessBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine($"var ret = new {info.Class.Name}();"); //TODO recheck if possible to support type args

        writer.WriteLine("uint k = 0;");
        writer.WriteLine($"foreach (var v in input.Tokenize('{info.Class.FieldSeparator}'))");
        using (writer.WriteBlock())
        {
            WriteThrowingLogic(writer, info);

            writer.WriteLine("key++;");
        }

        writer.WriteLine("return ret;");
    }

    public static void WriteThrowingLogic(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine("try");
        using (writer.WriteBlock())
        {

        }
        writer.WriteLine($$"""
            catch (Exception e)
            {
                throw new SerializationException(k, v, e);
            }
            """, true);
    }
}
