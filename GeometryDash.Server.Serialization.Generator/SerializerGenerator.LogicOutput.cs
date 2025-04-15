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
            namespace {{c.Namespace}};

            partial {{c.Declarator}} : global::{{KnownTypes.ISerializable}}<{{c.Name}}>
            """, true);

        using (writer.WriteBlock())
        {
            writer.WriteLine($"public static {c.Name} Deserialize(global::System.ReadOnlySpan<byte> input)");
            using (writer.WriteBlock())
            {
                writer.WriteLine($"var ret = new {info.Class.Name}();"); //TODO recheck if possible to support type args

                if (c.Keyed)
                    WriteKeyedBody(writer, info);
                else
                    WriteKeylessBody(writer, info);

                writer.WriteLine("return ret;");
            }

            foreach (var prop in info.Props)
            {
                writer.WriteLine();
                WriteDeserialization(writer, prop);
            }
        }

        ctx.AddSource($"{c.Namespace}.{c.Name}.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
    }

    public static void WriteKeyedBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine($"foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) {{ Separator = (byte)'{info.Class.PropSeparator}' }})");
        using (writer.WriteBlock())
        {
            WriteThrowingLogic(writer, info);
        }
    }

    public static void WriteKeylessBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine("uint key = 1;");
        writer.WriteLine($"foreach (var value in global::CommunityToolkit.HighPerformance.ReadOnlySpanExtensions.Tokenize(input, '{info.Class.PropSeparator}'))");
        using (writer.WriteBlock())
        {
            WriteThrowingLogic(writer, info);

            writer.WriteLine("key++;");
        }
    }

    public static void WriteThrowingLogic(IndentedTextWriter writer, SerializableClassInfo info)
    {
        //writer.WriteLine("try"); //TODO what even can throw? use results?
        //using (writer.WriteBlock())
        //{
        writer.WriteLine("switch (key)");
        using (writer.WriteBlock())
        {
            foreach (var prop in info.Props)
                writer.WriteLine($"case {prop.Index}: Deserialize{prop.Name}(value); break;");
            //don't throw on unrecognized keys to maintain forward-compat //TODO option to disable (for server api monitoring)
        }
        //}
        //writer.WriteLine($$"""
        //    catch (global::System.Exception e)
        //    {
        //        throw new global::GeometryDash.Server.Serialization.SerializationException(k, v, e);
        //    }
        //    """, true);
    }

    public static void WriteDeserialization(IndentedTextWriter writer, Prop prop)
    {
        writer.WriteLine($"void Deserialize{prop.Name}(global::System.ReadOnlySpan<byte> input)");
        using (writer.WriteBlock())
        {
            writer.WriteLine($"On{prop.Name}Deserializing(input);");
            //read attributes from end, since they're in serialization order, not deserialization order
            for (int i = prop.Transforms.Length - 1; i >= 0; i--)
            {
                var transform = prop.Transforms[i];
                switch (transform)
                {
                    //case Transform.Xor:
                    //    break;
                    //case Transform.Base64:
                }

                writer.WriteLine($"//{transform.ToString()}");
            }

            //Prop = value;
            //writer.WriteLine($"On{prop.Name}Deserialized({});");
            writer.WriteLine($"On{prop.Name}Deserialized();");
        }

        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserializing(global::System.ReadOnlySpan<byte> input);");
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized({prop.Type} value);");
    }

    public static void WritePostDeserializationValidation()
    {

    }
}
