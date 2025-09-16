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
            writer.WriteLine($"""
                var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);

                On{prop.Name}Deserializing(buffer);
                """, true);

            //read attributes from end, since they're in serialization order, not deserialization order
            var lastSpanVar = "input";
            var cleanup = new List<string>();
            for (int i = prop.Transforms.Length - 1; i >= 0; i--)
            {
                var transform = prop.Transforms[i];
                writer.WriteLine($"//{transform.ToString()}");
                switch (transform)
                {
                    //case Transform.Xor:
                    //    break;
                    case Transform.Base64:
                        lastSpanVar = WriteBase64Deserialization(writer, i, lastSpanVar, cleanup);
                        break;
                }
            }

            if (prop.Transforms.Length > 0)
            {
                writer.WriteLine();
                writer.WriteLine($"On{prop.Name}Deserialized(buffer);");
            }

            WriteBytesToValueConversion();

            if (prop.Transforms.Length > 0)
                writer.WriteLine("buffer.Dispose();");

            writer.WriteLine($"On{prop.Name}Deserialized();");
        }

        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserializing(scoped global::System.ReadOnlySpan<byte> input);");
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized(scoped global::System.ReadOnlySpan<byte> output);");
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized();");
    }

    private static string WriteBase64Deserialization(IndentedTextWriter writer, int i, string lastSpanVar, List<string> cleanup)
    {
        const int StackallocTreshold = 512;
        writer.WriteLine($$"""
            byte[]? t{{i}}_decodedArray = null;
            scoped global::System.Span<byte> t{{i}}_decoded;

            var t{{i}}_maxLength = global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength({{lastSpanVar}}.Length);
            if (t{{i}}_maxLength > {{StackallocTreshold}})
            {
                t{{i}}_decodedArray = global::System.Buffers.ArrayPool<byte>.Shared.Rent(t{{i}}_maxLength);
                t{{i}}_decoded = global::System.MemoryExtensions.AsSpan(t{{i}}_decodedArray);
            }
            else
                t{{i}}_decoded = stackalloc byte[{{StackallocTreshold}}];

            var t{{i}}_status = global::GeometryDash.Server.Serialization.Base64.DecodeCore({{lastSpanVar}}, t{{i}}_decoded, out _, out var t{{i}}_written);

            if (t{{i}}_status != global::System.Buffers.OperationStatus.Done)
            {
                if (t{{i}}_decodedArray is not null)
                    global::System.Buffers.ArrayPool<byte>.Shared.Return(t{{i}}_decodedArray);
                throw new global::System.ArgumentException($"Operation status indicates failure: {t{{i}}_status}");
            }

            var t{{i}} = t{{i}}_decoded[..t{{i}}_written];
            """, true);

        cleanup.Add($"""
            if (t{i}_decodedArray is not null)
                global::System.Buffers.ArrayPool<byte>.Shared.Return(t{i}_decodedArray);
            """);
        return $"t{i}";
    }

    public static void WritePostDeserializationValidation()
    {

    }
}
