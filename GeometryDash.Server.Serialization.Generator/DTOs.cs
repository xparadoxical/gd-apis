using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

public record struct SerializableClassInfo(Class Class, EquatableArray<Prop> Props);
public sealed record Class(string Namespace, string Name, string Declarator, string PropSeparator, string ListSeparator, bool Keyed);
/// <param name="Type">Fully qualified type name.</param>
public sealed record Prop(string Type, PropTypeInfo ParsedType, bool Required, string Name, uint Index, BoolSpec? BoolSpec, EquatableArray<Transform> Transforms,
    EquatableArray<string> ToNull, string? FromEmpty);

/// <param name="ConstructedFrom">
/// For e.g. <c>System.UInt32</c>, the value is <see cref="uint"/>.
/// Returns <see langword="null"/> when <paramref name="Type"/> is an array, a pointer or a type parameter.
/// </param>
public sealed record PropTypeInfo(bool Nullable, string Type, SpecialType SpecialType, string? ConstructedFrom)
{
    public static bool TryCreate(PropertyDeclarationSyntax prop, SemanticModel sm, [NotNullWhen(true)] out PropTypeInfo? result)
    {
        var typeSyntax = prop.Type;

        var nullable = false;
        if (typeSyntax is NullableTypeSyntax nullableType)
        {
            typeSyntax = nullableType.ElementType;
            nullable = true;
        }

        if (sm.GetTypeInfo(typeSyntax) is not { Type: { } typeSymbol })
        {
            result = default;
            return false;
        }

        //useful: typeSymbol.Is*, typeSymbol.ToDisplayParts() //TODO wtf did I mean?
        result = new(nullable, typeSyntax.ToString(), typeSymbol.SpecialType, (typeSymbol as INamedTypeSymbol)?.ConstructedFrom.Name);
        return true;
    }
}

public record struct BoolSpec(string TrueExpr, string? FalseExpr);

public abstract record Transform
{
    public sealed record Base64 : Transform;
    public sealed record Gzip : Transform;
    public sealed record Xor(string KeyExpr) : Transform;
}
