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
            #nullable enable
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
                WriteDeserializationMethods(writer, prop);
            }
        }

        ctx.AddSource($"{c.Namespace}.{c.Name}.g.cs", SourceText.From(writer.ToString(), Encoding.UTF8));
    }

    public static void WriteKeyedBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine($"foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) {{ Separator = (byte)'{info.Class.PropSeparator}' }})");
        using (writer.WriteBlock())
        {
            WritePropertySwitch(writer, info);
        }
    }

    public static void WriteKeylessBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine("var key = 1;");
        writer.WriteLine($"foreach (var value in global::CommunityToolkit.HighPerformance.ReadOnlySpanExtensions.Tokenize(input, '{info.Class.PropSeparator}'))");
        using (writer.WriteBlock())
        {
            WritePropertySwitch(writer, info);

            writer.WriteLine("key++;");
        }
    }

    public static void WritePropertySwitch(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine("switch (key)");
        using (writer.WriteBlock())
        {
            foreach (var prop in info.Props)
                writer.WriteLine($"case {prop.Index}: ret.Deserialize{prop.Name}(value); break;");
            //don't throw on unrecognized keys to maintain forward-compat //TODO option to disable (for server api monitoring)
        }
    }

    public static void WriteDeserializationMethods(IndentedTextWriter writer, SerializableProperty prop)
    {
        writer.WriteLine($"void Deserialize{prop.Name}(global::System.ReadOnlySpan<byte> input)");
        using (writer.WriteBlock())
        {
            if (prop is not { OnDeserializingHooked: false, Transforms: [] })
            {
                writer.WriteLine($"""
                    var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);

                    On{prop.Name}Deserializing(buffer);

                    """, true);
            }

            //read attributes from end, since they're in serialization order, not deserialization order
            for (int i = prop.Transforms.Length - 1; i >= 0; i--)
            {
                var transform = prop.Transforms[i];
                switch (transform)
                {
                    case Transform.Base64:
                        writer.WriteLine("""
                            buffer.EnsureCapacity(global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength(buffer.DataLength));
                            buffer.DataLength = global::GeometryDash.Server.Serialization.Base64.Decode(buffer.DataSpan);
                            """, true);
                        break;

                    case Transform.Xor xor:
                        writer.WriteLine($"""global::GeometryDash.Server.Serialization.Xor.Apply(buffer.DataSpan, {xor.KeyExpr}u8);""");
                        break;

                    case Transform.Gzip:
                        writer.WriteLine($"""
                            var t{i}_length = global::GeometryDash.Server.Serialization.Gzip.GetDecompressedLength(buffer.DataSpan);
                            var t{i}_output = global::System.Buffers.ArrayPool<byte>.Shared.Rent((int)t{i}_length);
                            global::GeometryDash.Server.Serialization.Gzip.Decompress(buffer.DataSpan, t{i}_output);
                            buffer.DataLength = t{i}_output.Length;
                            global::System.MemoryExtensions.AsSpan(t{i}_output).CopyTo(buffer.DataSpan);
                            global::System.Buffers.ArrayPool<byte>.Shared.Return(t{i}_output);
                            """, true);
                        break;

                    default:
                        throw new NotImplementedException(transform.ToString());
                }
                writer.WriteLine();
            }

            if (prop.Transforms is not [])
            {
                writer.WriteLine($"On{prop.Name}Deserialized(buffer);");
                writer.WriteLine();
            }

            WriteBytesToValueConversion(); //includes tonull and emptyto handling
            writer.WriteLine();

            if (prop.Transforms is not [])
                writer.WriteLine("buffer.Dispose();");

            writer.WriteLine($"On{prop.Name}Deserialized();");
        }

        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserializing(global::PoolBuffers.PooledBuffer<byte> input);");
        if (prop.Transforms is not [])
        {
            writer.WriteLine();
            writer.WriteLine($"partial void On{prop.Name}Deserialized(global::PoolBuffers.PooledBuffer<byte> output);");
        }
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized();");
    }
}
