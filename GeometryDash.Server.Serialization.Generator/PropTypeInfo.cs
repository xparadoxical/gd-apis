using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

public sealed record ElementTypeInfo(string Fqn, TypeKind Kind, SpecialType SpecialType, bool IsINumberBase,
    bool IsISerializable, bool IsListType);

/// <param name="ElementType">The type of elements of an array or an enumerable type.</param>
public sealed record PropTypeInfo(bool Nullable, string Fqn, ElementTypeInfo ElementType, string? ElementSeparator,
    TypeKind Kind, SpecialType SpecialType, bool Optional)
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

        //unwrap Optional<T>
        var isOptional = false;
        if (typeSymbol is INamedTypeSymbol
            {
                IsGenericType: true,
                TypeArguments: [var optionalArg],
                Name: "Optional",
                ContainingNamespace: { Name: "Serialization", ContainingNamespace: { Name: "Server", ContainingNamespace: { Name: "GeometryDash", ContainingNamespace.IsGlobalNamespace: true } } }
            })
        {
            isOptional = true;
            typeSymbol = optionalArg;
        }

        //extract element type - the T in T[], List<T>, etc
        var isListType = false;
        ITypeSymbol? elementTypeSymbol = null;
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            elementTypeSymbol = arrayTypeSymbol.ElementType;
            isListType = true;
        }

        var propertyTypeFQN = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var coreType = elementTypeSymbol ?? typeSymbol;
        var elementTypeFQN = coreType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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

        result = new(nullable, propertyTypeFQN,
            new(elementTypeFQN ?? propertyTypeFQN, coreType.TypeKind,
            coreType.SpecialType, implementsINumberBase, implementsISerializable, isListType),
            typeProvidedElementSeparator, typeSymbol.TypeKind, typeSymbol.SpecialType, isOptional);
        return true;
    }
}
