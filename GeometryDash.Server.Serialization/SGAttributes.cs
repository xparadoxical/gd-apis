namespace GeometryDash.Server.Serialization;

file static class SupportedTargets
{
    internal const AttributeTargets Types = AttributeTargets.Class | AttributeTargets.Struct;
    internal const AttributeTargets Members = AttributeTargets.Property;
}

//#pragma warning disable CS9113

/// <summary>The type is serialized with field indexes as keys.</summary>
[AttributeUsage(SupportedTargets.Types, Inherited = false)]
public sealed class KeyedAttribute : Attribute;

[AttributeUsage(SupportedTargets.Types, Inherited = false)]
public sealed class SeparatorAttribute : Attribute
{
    public required string Field { get; init; }
    public required string ListItem { get; init; }
}

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class FieldAttribute(uint index) : Attribute;

/// <summary>
/// Specifies what value indicates <see langword="true"/>.
/// If <see cref="False"/> is not set, any value other than <paramref name="trueString"/> means <see langword="false"/>.
/// </summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class BoolAttribute(string trueString) : Attribute
{
    public string TrueString { get; } = trueString;
    /// <summary>The value that indicates <see langword="false"/>.</summary>
    public string? False { get; init; }
}

/// <summary>The specified value will be converted to <see langword="null"/>.</summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false, AllowMultiple = true)]
public sealed class CoalesceToNullAttribute(object fromValue) : Attribute;

/// <summary>An empty string will be replaced with <paramref name="defaultValue"/>.</summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class EmptyDefaultsToAttribute(object defaultValue) : Attribute;

public abstract class DataTransformAttribute : Attribute;

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class Base64EncodedAttribute : DataTransformAttribute;

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class XorAttribute : DataTransformAttribute
{
    public XorAttribute(string key) { }
    public XorAttribute(ReadOnlySpan<byte> key) { }
}

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class GzipAttribute : DataTransformAttribute;
