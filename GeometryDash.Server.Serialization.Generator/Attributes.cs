namespace GeometryDash.Server.Serialization.Generator;

file static class Consts
{
    internal const AttributeTargets Types = AttributeTargets.Class | AttributeTargets.Struct;
    internal const AttributeTargets Members = AttributeTargets.Property;
}

//#pragma warning disable CS9113

/// <summary>The type is serialized with field indexes as keys.</summary>
[AttributeUsage(Consts.Types, Inherited = false)]
internal sealed class KeyedAttribute : Attribute;

[AttributeUsage(Consts.Types, Inherited = false)]
internal sealed class SeparatorAttribute : Attribute
{
    public required string Field { get; init; }
    public required string ListItem { get; init; }
}

[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class FieldAttribute(uint index) : Attribute;

/// <summary>
/// Specifies what value indicates <see langword="true"/>.
/// If <see cref="False"/> is not set, any value other than <paramref name="trueString"/> is <see langword="false"/>.
/// </summary>
[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class BoolAttribute(string trueString) : Attribute
{
    public string TrueString { get; } = trueString;
    /// <summary>The value that indicates <see langword="false"/>.</summary>
    public string? False { get; init; }
}

/// <summary>The specified value will be converted to <see langword="null"/>.</summary>
[AttributeUsage(Consts.Members, Inherited = false, AllowMultiple = true)]
internal sealed class CoalesceToNullAttribute(object fromValue) : Attribute;

/// <summary>An empty string will be replaced with <paramref name="defaultValue"/>.</summary>
[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class EmptyDefaultsToAttribute(object defaultValue) : Attribute;

internal abstract class DataTransformAttribute : Attribute;

[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class Base64EncodedAttribute : DataTransformAttribute;

[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class XorAttribute : DataTransformAttribute
{
    public XorAttribute(string key) { }
    public XorAttribute(ReadOnlySpan<byte> key) { }
}

[AttributeUsage(Consts.Members, Inherited = false)]
internal sealed class GzipAttribute : DataTransformAttribute;
