using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

public record struct SerializableClassInfo(SerializableClass Class, EquatableArray<SerializableProperty> Props);

public sealed record SerializableClass(string Namespace, string Name, string Declarator, string PropSeparator,
    string ListSeparator, bool Keyed);

/// <param name="Type">Fully qualified type name.</param>
public sealed record SerializableProperty(string Type, PropTypeInfo ParsedType, bool Required, string Name,
    uint Index, BoolSpec? BoolSpec, EquatableArray<Transform> Transforms, EquatableArray<string> ToNull,
    string? FromEmpty, bool OnDeserializingHooked);

/// <param name="Type"></param>
/// <param name="ElementType">The type of elements of an array or an enumerable type.</param>
/// <param name="ConstructedFrom">
/// For e.g. <c>System.UInt32</c>, the value is <see cref="uint"/>.
/// Returns <see langword="null"/> when <paramref name="Type"/> is an array, a pointer or a type parameter.
/// </param>
public sealed record PropTypeInfo(bool Nullable, string Type, string? ElementType, TypeKind Kind,
    SpecialType SpecialType, bool IsINumberBase)
{
    public static bool TryCreate(PropertyDeclarationSyntax prop, SemanticModel sm,
        [NotNullWhen(true)] out PropTypeInfo? result)
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

        TypeSyntax? collectionElementType = null;
        if (typeSyntax is ArrayTypeSyntax arrayType)
        {
            collectionElementType = arrayType.ElementType;
        }
        //TODO enumerable types

        ITypeSymbol? collectionElementTypeSymbol = null;
        if (collectionElementType is not null && sm.GetTypeInfo(collectionElementType) is { Type: { } colElTypeSymbol })
            collectionElementTypeSymbol = colElTypeSymbol;

        result = new(nullable, typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            collectionElementTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            typeSymbol.TypeKind, typeSymbol.SpecialType,
            typeSymbol.AllInterfaces.Any(sym => sym.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.Numerics.INumberBase<"))
            );
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
