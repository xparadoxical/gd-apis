using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace GeometryDash.Server.Serialization.Generator;

public record struct SerializableClassInfo(SerializableClass Class, EquatableArray<SerializableProperty> Props);

public sealed record SerializableClass(string Namespace, string Name, string Declarator, string PropSeparator,
    string ListSeparator, bool Keyed);

/// <param name="Type">Fully qualified type name.</param>
public sealed record SerializableProperty(PropTypeInfo ParsedType, bool Required, string Name,
    string? Index, BoolSpec? BoolSpec, EquatableArray<Transform> Transforms, EquatableArray<string> ToNull,
    string? FromEmpty, bool OnDeserializingHooked, string? ElementSeparatorOverride)
{
    public string? EffectiveElementSeparator => ElementSeparatorOverride ?? ParsedType.ElementSeparator;
}

public record struct BoolSpec(string? TrueExpr, string? FalseExpr);

public abstract record Transform
{
    public sealed record Base64 : Transform;
    public sealed record Gzip : Transform;
    public sealed record Xor(string KeyExpr) : Transform;
}
