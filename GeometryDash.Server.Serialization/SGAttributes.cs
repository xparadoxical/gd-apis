using System.Diagnostics.CodeAnalysis;

namespace GeometryDash.Server.Serialization;

file static class SupportedTargets
{
    internal const AttributeTargets Types = AttributeTargets.Class/* | AttributeTargets.Struct*/;
    internal const AttributeTargets Members = AttributeTargets.Property;
}

#pragma warning disable CS9113
#pragma warning disable CS8618

/// <summary>The type is serialized with prop indexes as keys.</summary>
[AttributeUsage(SupportedTargets.Types, Inherited = false)]
public sealed class KeyedAttribute : Attribute;

[AttributeUsage(SupportedTargets.Types | SupportedTargets.Members, Inherited = false)]
public sealed class SeparatorAttribute : Attribute
{
    public SeparatorAttribute() { }
    [SetsRequiredMembers] //lies... but list properties can only specify item separator
    public SeparatorAttribute(string itemSeparator) { }

    public required string Prop { get; init; }
    public required string ListItem { get; init; }
}

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class IndexAttribute(uint index) : Attribute;

/// <summary>
/// Specifies what values indicate <see langword="true"/> and <see langword="false"/>.
/// If a property is not set, it corresponds to any value different from the one specified for the other property.
/// </summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class BoolAttribute : Attribute
{
    public string True { get; init; }
    public string False { get; init; }
}

/// <summary>The specified value will be converted to <see langword="null"/>.</summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false, AllowMultiple = true)]
public sealed class CoalesceToNullAttribute(object fromValue) : Attribute;

/// <summary>An empty string will be replaced with <paramref name="defaultValue"/>.</summary>
[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class EmptyDefaultsToAttribute(object defaultValue) : Attribute;

public abstract class DataTransformAttribute : Attribute;

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class Base64EncodeAttribute : DataTransformAttribute;

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class XorAttribute : DataTransformAttribute
{
    public XorAttribute(string key) { }
    public XorAttribute(ReadOnlySpan<byte> key) { }
}

[AttributeUsage(SupportedTargets.Members, Inherited = false)]
public sealed class GzipAttribute : DataTransformAttribute;
