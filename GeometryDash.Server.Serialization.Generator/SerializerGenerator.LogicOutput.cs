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
            var lastSpanVar = "input";
            var cleanup = new List<string>();
            for (int i = prop.Transforms.Length - 1; i >= 0; i--)
            {
                var transform = prop.Transforms[i];
                writer.WriteLine($"//{transform.ToString()}");
                //TODO use throw helpers or switch to results (or use ExceptionDispatchInfo.SetCurrentStackTrace to return an exception to throw later)
                switch (transform)
                {
                    //case Transform.Xor:
                    //    break;
                    case Transform.Base64:
                        lastSpanVar = WriteBase64Deserialization(writer, i, lastSpanVar, cleanup);
                        break;
                }
            }

            writer.WriteLine($"On{prop.Name}Deserialized({lastSpanVar});");

            //Prop = {currentDataVar%conversion};

            foreach (var code in cleanup)
                writer.WriteLine(code);

            writer.WriteLine($"On{prop.Name}Deserialized();");
        }

        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserializing(global::System.ReadOnlySpan<byte> input);");
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized(global::System.ReadOnlySpan<byte> output);");
        writer.WriteLine();
        writer.WriteLine($"partial void On{prop.Name}Deserialized({prop.Type} value);");
    }

    private static string WriteBase64Deserialization(IndentedTextWriter writer, int i, string lastSpanVar, List<string> cleanup)
    {
        const int StackallocTreshold = 512;
        writer.WriteLine($$"""
            byte[]? t{{i}}_decodedArray = null;
            scoped global::System.Span<byte> t{{i}}_decoded;

            var t{{i}}_maxLength = global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength({{lastSpanVar}});
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
