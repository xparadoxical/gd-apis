using System.Diagnostics.CodeAnalysis;

using GeometryDash.Server.Serialization.Generator.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

public record struct SerializableClassInfo(Class Class, EquatableArray<Field> Fields);
public sealed record Class(string Namespace, string Name, string Declarator, string FieldSeparator, string ListSeparator, bool Keyed);
public sealed record Field(FieldTypeInfo Type, bool Required, string Name, uint Index, BoolSpec? BoolSpec, EquatableArray<Transform> Transforms,
    EquatableArray<string> ToNull, string? FromEmpty);

/// <param name="ConstructedFrom">
/// For e.g. <c>System.UInt32</c>, the value is <see cref="uint"/>.
/// Returns <see langword="null"/> when <paramref name="Type"/> is an array, a pointer or a type parameter.
/// </param>
public sealed record FieldTypeInfo(bool Nullable, string Type, SpecialType SpecialType, string? ConstructedFrom)
{
    public static bool TryCreate(PropertyDeclarationSyntax prop, SemanticModel sm, [NotNullWhen(true)] out FieldTypeInfo? result)
    {
        var typeSyntax = prop.Type;

        var nullable = false;
        if (typeSyntax is NullableTypeSyntax nullableType)
        {
            typeSyntax = nullableType.ElementType;
            nullable = true;
        }

        if (sm.GetTypeInfo(typeSyntax) is not { Type: { } typeSymbol })
            goto fail;

        //useful: typeSymbol.Is*, typeSymbol.ToDisplayParts()
        result = new(nullable, typeSyntax.ToString(), typeSymbol.SpecialType, (typeSymbol as INamedTypeSymbol)?.ConstructedFrom.Name);
        return true;

    fail:
        result = default;
        return false;
    }
}

public record struct BoolSpec(string True, string? False);

public abstract record Transform
{
    public sealed record Base64 : Transform;
    public sealed record Gzip : Transform;
    public sealed record Xor(string Key, string ExpressionType /*TODO check if needed*/) : Transform;
}
