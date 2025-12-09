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
public sealed record SerializableProperty(PropTypeInfo ParsedType, bool Required, string Name,
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
    TypeKind Kind, SpecialType SpecialType, SpecialType ElementSpecialType, bool ElementIsINumberBase, bool ElementIsISerializable,
    bool Optional, bool IsListType)
{
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

        //extract element type - the T in T[], List<T>, Optional<T>, etc
        var isListType = false;
        ITypeSymbol? elementTypeSymbol = null;
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            elementTypeSymbol = arrayTypeSymbol.ElementType;
            isListType = true;
        }
        else if (typeSymbol is INamedTypeSymbol { } namedTypeSymbol)
        {
            var namespaceQualifiedName = namedTypeSymbol.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                    .WithGenericsOptions(SymbolDisplayGenericsOptions.None));

            if (namespaceQualifiedName == KnownTypes.Optional)
                elementTypeSymbol = namedTypeSymbol.TypeArguments.FirstOrDefault();
        }

        var propertyTypeFQN = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var elementTypeFQN = elementTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var coreType = elementTypeSymbol ?? typeSymbol;
        var typeProvidedElementSeparator = coreType.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == KnownTypes.SeparatorAttribute)
            ?.NamedArguments.SingleOrNullable(kvp => kvp.Key == "ListItem")
            ?.Value.ToCSharpString();

        //generated partial declarations with ISerializable implementation are not detectable here,
        //but if SeparatorAttr is used, the type will be serializable
        var implementsINumberBase = false;
        var implementsISerializable = typeProvidedElementSeparator is not null;
        coreType.AllInterfaces
            .Select(sym => sym.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .MultiFirst(
                (fqn => fqn.StartsWith("global::System.Numerics.INumberBase<"),
                    _ => implementsINumberBase = true),
                (fqn => fqn.StartsWith("global::" + KnownTypes.ISerializable),
                    _ => implementsISerializable = true));

        var isOptional = propertyTypeFQN.StartsWith("global::" + KnownTypes.Optional);

        result = new(nullable, propertyTypeFQN, elementTypeFQN, typeProvidedElementSeparator,
            typeSymbol.TypeKind, typeSymbol.SpecialType, coreType.SpecialType, implementsINumberBase, implementsISerializable,
            isOptional, isListType);
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
