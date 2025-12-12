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

            writer.WriteLine();
            WritePropertySelector(writer, info);

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
        writer.WriteLine($"foreach (var (key, value) in new global::GeometryDash.Server.Serialization.RobTopStringReader(input) {{ Separator = {info.Class.PropSeparator}u8 }})");
        using (writer.WriteBlock())
        {
            writer.WriteLine("try");
            using (writer.WriteBlock())
            {
                writer.WriteLine("if (!PropertySelector(key, value, ret))");
                using (writer.WriteBlock())
                {
                    //don't throw on unrecognized keys to maintain forward-compat //TODO option to disable (for server api monitoring)
                }
            }
            writer.WriteLine("catch (global::System.Exception ex)");
            using (writer.WriteBlock())
            {
                writer.WriteLine($"throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);");
            }
        }
    }

    public static void WriteKeylessBody(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine("var key = 1u;");
        writer.WriteLine($"foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>(input, {info.Class.PropSeparator}u8))");
        using (writer.WriteBlock())
        {
            writer.WriteLine("try");
            using (writer.WriteBlock())
            {
                writer.WriteLine("if (!PropertySelector(key, value, ret))");
                using (writer.WriteBlock())
                {
                    //don't throw on unrecognized keys to maintain forward-compat
                }
            }
            writer.WriteLine("catch (global::System.Exception ex)");
            using (writer.WriteBlock())
            {
                writer.WriteLine($"throw new global::GeometryDash.Server.Serialization.SerializationException(key, ex);");
            }

            writer.WriteLine("key++;");
        }
    }

    public static void WritePropertySelector(IndentedTextWriter writer, SerializableClassInfo info)
    {
        writer.WriteLine($"internal static bool PropertySelector(uint key, global::System.ReadOnlySpan<byte> value, {info.Class.Name} ret)");
        using (writer.WriteBlock())
        {
            writer.WriteLine("switch (key)");
            using (writer.WriteBlock())
            {
                foreach (var prop in info.Props)
                    writer.WriteLine($"case {prop.Index}: ret.Deserialize{prop.Name}(value); return true;");
            }

            if (info.Class.BaseClassFqn is not null)
            {
                writer.WriteLine($"return {info.Class.BaseClassFqn}.PropertySelector(key, value, ret);");
            }
            else
            {
                writer.WriteLine("return false;");
            }
        }
    }

    public static void WriteDeserializationMethods(IndentedTextWriter writer, SerializableProperty prop)
    {
        writer.WriteLine($"void Deserialize{prop.Name}(global::System.ReadOnlySpan<byte> input)");
        using (writer.WriteBlock())
        {
            var usePooledBuffer = prop is not { OnDeserializingHooked: false, Transforms: [] };

            if (usePooledBuffer)
            {
                writer.WriteLine($"""
                    var buffer = new global::PoolBuffers.PooledBuffer<byte>(input);

                    On{prop.Name}Deserializing(buffer);

                    """, true);
            }

            WriteTransforms(writer, prop);

            WritePostTransformationConversion(writer, prop); //includes tonull and emptyto handling

            if (usePooledBuffer)
                writer.WriteLine("buffer.Dispose();");

            writer.WriteLine($"On{prop.Name}Deserialized();");
        }

        //OnDeserializing always needs to be generated for it to be available in partial method suggestions.
        //It requires a PooledBuffer, so when it's implemented, the "only use the input ROS" optimization gets disabled.
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

    private static void WriteTransforms(IndentedTextWriter writer, SerializableProperty prop)
    {
        //read attributes from end, since they're in serialization order, not deserialization order
        for (int i = prop.Transforms.Length - 1; i >= 0; i--)
        {
            var transform = prop.Transforms[i];
            switch (transform)
            {
                case Transform.Base64:
                    writer.WriteLine("""
                            buffer.EnsureCapacity(global::GeometryDash.Server.Serialization.Base64.GetMaxDecodedLength(buffer.Length));
                            buffer.Length = global::GeometryDash.Server.Serialization.Base64.Decode(buffer.DataSpan);
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
                            buffer.Length = t{i}_output.Length;
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
    }

    public static void WritePostTransformationConversion(IndentedTextWriter writer, SerializableProperty prop)
    {
        var usePooledBuffer = prop is not { OnDeserializingHooked: false, Transforms: [] };
        var includeEmptyInputPreambule = prop is { FromEmpty: not null } or { ParsedType.Optional: true };
        var spanExpr = usePooledBuffer ? "buffer.DataSpan" : "input";

        if (includeEmptyInputPreambule)
        {
            writer.Write($"if (");
            writer.Write(usePooledBuffer ? "buffer.Length == 0" : "input.IsEmpty");
            writer.WriteLine(")");

            writer.IncreaseIndent();
            var defaultValue = prop.ParsedType.Optional
                ? $"global::GeometryDash.Server.Serialization.Optional<{prop.ParsedType.Fqn}>.Empty"
                : prop.FromEmpty;
            writer.WriteLine($"{prop.Name} = {defaultValue};");
            writer.DecreaseIndent();

            writer.WriteLine("else");
        }

        if (!prop.ParsedType.ElementType.IsListType)
        {
            if (includeEmptyInputPreambule)
                writer.IncreaseIndent();
            writer.Write($"{prop.Name} = ");

            if (prop.ParsedType.SpecialType == SpecialType.System_String)
            {
                writer.WriteLine($"global::System.Text.Encoding.UTF8.GetString({spanExpr});"); //TODO string.Create+Utf8.ToUtf16
            }
            else if (prop.ParsedType.ElementType.IsINumberBase)
            {
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<{prop.ParsedType.ElementType.Fqn}>({spanExpr});");
            }
            else if (prop.ParsedType.Fqn == "global::System.TimeSpan")
            {
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ParsingExtensions.ParseTimeSpan({spanExpr});");
            }
            else if (prop.ParsedType.Kind == TypeKind.Enum)
            {
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ParsingExtensions.ParseEnum<{prop.ParsedType.ElementType.Fqn}>({spanExpr});");
            }
            else if (prop is { ParsedType.ElementType.SpecialType: SpecialType.System_Boolean, BoolSpec: BoolSpec(var trueExpr, var falseExpr) })
            {
                var trueArg = trueExpr is null ? "new()" : $"{trueExpr}u8";
                var falseArg = falseExpr is null ? "new()" : $"{falseExpr}u8";
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ParsingExtensions.ParseBool({spanExpr}, {trueArg}, {falseArg});");
            }
            else if (prop.ParsedType.ElementType.IsISerializable)
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ServerSerializer.DeserializeSerializable<{prop.ParsedType.ElementType.Fqn}>({spanExpr});");

            if (includeEmptyInputPreambule)
                writer.DecreaseIndent();
        }
        else
        {
            if (includeEmptyInputPreambule)
            {
                writer.WriteLine("{");
                writer.IncreaseIndent();
            }

            writer.Write("var ret = ");

            if (prop.ParsedType.Kind == TypeKind.Array)
                writer.WriteLine($"new {prop.ParsedType.ElementType.Fqn}[global::System.MemoryExtensions.Count({spanExpr}, {prop.EffectiveElementSeparator}u8) + 1];");
            //TODO other collection types

            writer.WriteLine($"""
                int i = 0;
                foreach (var value in new global::CommunityToolkit.HighPerformance.Enumerables.ReadOnlySpanTokenizerWithSpanSeparator<byte>({spanExpr}, {prop.EffectiveElementSeparator}u8))
                """, true);

            writer.IncreaseIndent();

            if (prop.ParsedType.Kind == TypeKind.Array)
                writer.Write("ret[i++] = ");

            if (prop.ParsedType.ElementType.IsISerializable)
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ServerSerializer.DeserializeSerializable<{prop.ParsedType.ElementType.Fqn}>(value);");
            else if (prop.ParsedType.ElementType.IsINumberBase)
                writer.WriteLine($"global::GeometryDash.Server.Serialization.ParsingExtensions.Parse<{prop.ParsedType.ElementType.Fqn}>(value);");

            writer.DecreaseIndent();

            writer.WriteLine($"{prop.Name} = ret;");

            if (includeEmptyInputPreambule)
            {
                writer.DecreaseIndent();
                writer.WriteLine("}");
            }
        }

        writer.WriteLine();

        if (prop.ToNull is not [])
        {
            writer.WriteLine($"if ({prop.Name} is {string.Join(" or ", prop.ToNull)})");
            writer.IncreaseIndent();
            writer.WriteLine($"{prop.Name} = null;");
            writer.DecreaseIndent();
            writer.WriteLine();
        }
    }
}
