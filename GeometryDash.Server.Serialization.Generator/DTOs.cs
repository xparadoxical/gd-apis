using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

public record struct SerializableClassInfo(SerializableClass Class, EquatableArray<SerializableProperty> Props);

public sealed record SerializableClass(string Namespace, string Name, string Declarator, string PropSeparator,
    string ListSeparator, bool Keyed);

/// <param name="Type">Fully qualified type name.</param>
public sealed record SerializableProperty(string Type, PropTypeInfo ParsedType, bool Required, string Name,
    string? Index, BoolSpec? BoolSpec, EquatableArray<Transform> Transforms, EquatableArray<string> ToNull,
    string? FromEmpty, bool OnDeserializingHooked, string? ElementSeparatorOverride)
{
    public string? EffectiveElementSeparator => ElementSeparatorOverride ?? ParsedType.ElementSeparator;
}

/// <param name="Type"></param>
/// <param name="ElementType">The type of elements of an array or an enumerable type.</param>
/// <param name="ConstructedFrom">
/// For e.g. <c>System.UInt32</c>, the value is <see cref="uint"/>.
/// Returns <see langword="null"/> when <paramref name="Type"/> is an array, a pointer or a type parameter.
/// </param>
public sealed record PropTypeInfo(bool Nullable, string Type, string? ElementType, string? ElementSeparator,
    TypeKind Kind, SpecialType SpecialType, bool ElementIsINumberBase, bool ElementIsISerializable)
{
    public bool IsListType => ElementType is not null;

    public static bool TryCreate(PropertyDeclarationSyntax prop, SemanticModel sm,
        [NotNullWhen(true)] out PropTypeInfo? result)
    {
        var typeSyntax = prop.Type;

        //extract type from nullable
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

        ITypeSymbol? elementTypeSymbol = null;

        //extract element type - the T in T[], List<T>, Optional<T>, etc
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            elementTypeSymbol = arrayTypeSymbol.ElementType;

        var propertyTypeFQN = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var elementTypeFQN = elementTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var typeProvidedElementSeparator = (elementTypeSymbol ?? typeSymbol)?.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == KnownTypes.SeparatorAttribute)
            ?.NamedArguments.SingleOrNullable(kvp => kvp.Key == "ListItem")
            ?.Value.ToCSharpString();

        //generated partial declarations with ISerializable implementation are not detectable here,
        //but if SeparatorAttr is used, the type will be serializable
        var implementsINumberBase = false;
        var implementsISerializable = typeProvidedElementSeparator is not null;
        (elementTypeSymbol ?? typeSymbol).AllInterfaces
            .Select(sym => sym.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .MultiFirst(
                (fqn => fqn.StartsWith("global::System.Numerics.INumberBase<"),
                    _ => implementsINumberBase = true),
                (fqn => fqn.StartsWith("global::" + KnownTypes.ISerializable),
                    _ => implementsISerializable = true));

        result = new(nullable, propertyTypeFQN, elementTypeFQN, typeProvidedElementSeparator,
            typeSymbol.TypeKind, typeSymbol.SpecialType, implementsINumberBase, implementsISerializable);
        return true;
    }
}

public record struct BoolSpec(string? TrueExpr, string? FalseExpr);

public abstract record Transform
{
    public sealed record Base64 : Transform;
    public sealed record Gzip : Transform;
    public sealed record Xor(string KeyExpr) : Transform;
}
