using System.Reflection;

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

[Generator(LanguageNames.CSharp)]
public sealed partial class SerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var serializableTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
            KnownTypes.SeparatorAttribute,
            (syntax, ct) => syntax is ClassDeclarationSyntax,
            (ctx, ct) =>
            {
                var decl = ctx.TargetNode.Cast<ClassDeclarationSyntax>();
                return (c: GetClassInfo(ctx, decl, ct),
                        p: GetPropertyInfos(ctx, decl.Members, ct));
            })
            .Where(info => info.c is not null)
            .Select((info, ct) => new SerializableClassInfo(info.c!, info.p));

        context.RegisterSourceOutput(serializableTypes, GenerateType);
    }

    public static SerializableClass? GetClassInfo(GeneratorAttributeSyntaxContext ctx, ClassDeclarationSyntax decl, CancellationToken ct)
    {
        var separatorAttr = ctx.Attributes.Single(a => a.AttributeClass!.ToDisplayString() == KnownTypes.SeparatorAttribute);
        //TODO SingleOrDefault
        var propSeparator = SymbolDisplay.FormatPrimitive(
            separatorAttr.NamedArguments.Single(arg => arg.Key is "Prop").Value.Value!,
            true, false);
        var listSeparator = SymbolDisplay.FormatPrimitive(
            separatorAttr.NamedArguments.Single(arg => arg.Key is "ListItem").Value.Value!,
            true, false);

        var keyed = ctx.TargetSymbol.GetAttributes()
            .SingleOrDefault(a => a.AttributeClass!.ToDisplayString() == KnownTypes.KeyedAttribute) is not null;

        string? baseClassFqn = null;

        if (decl.BaseList is { Types: [var firstBase, ..] })
        {
            var baseTypeInfo = ctx.SemanticModel.GetTypeInfo(firstBase.Type, ct);
            if (baseTypeInfo.Type is { TypeKind: TypeKind.Class } baseClassSymbol)
            {
                baseClassFqn = baseClassSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        return new(
            decl.Parent!.Cast<BaseNamespaceDeclarationSyntax>().Name.ToString(),
            decl.Identifier.ToString(),
            $"{decl.Keyword} {decl.Identifier}{decl.TypeParameterList}",
            propSeparator, listSeparator, keyed, baseClassFqn);
    }

    /// <summary>Collects info about properties marked with <see cref="KnownTypes.IndexAttribute"/>.</summary>
    public static EquatableArray<SerializableProperty> GetPropertyInfos(GeneratorAttributeSyntaxContext ctx, SyntaxList<MemberDeclarationSyntax> members, CancellationToken ct)
    {
        var infos = new List<SerializableProperty>();

        foreach (var prop in members.OfType<PropertyDeclarationSyntax>())
        {
            string? index = null;
            BoolSpec? boolSpec = null;
            var transforms = new List<Transform>();
            var toNull = new List<string>();
            string? fromEmpty = null;
            string? separator = null;
            foreach (var attr in prop.AttributeLists.SelectMany(l => l.Attributes))
            {
                var type = ctx.SemanticModel.GetTypeInfo(attr, ct).Type;
                if (type is null)
                    continue;

                var attrFullName = type.ToDisplayString();
                if (attrFullName == KnownTypes.IndexAttribute)
                {
                    var arglist = attr.ArgumentList;
                    if (arglist is null)
                        continue;

                    if (arglist.Arguments is not [{ Expression: var expr }])
                        continue;

                    index = expr.ToString();
                }
                else if (attrFullName == KnownTypes.BoolAttribute)
                {
                    if (attr.ArgumentList is not
                        {
                            Arguments: [{ NameEquals: not null }]
                                or [{ NameEquals: not null }, { NameEquals: not null }]
                        })
                        continue;

                    AttributeArgumentSyntax? trueArg = null;
                    AttributeArgumentSyntax? falseArg = null;
                    foreach (var arg in attr.ArgumentList.Arguments)
                    {
                        if (arg.NameEquals!.Name.Identifier.Text is "True")
                            trueArg = arg;
                        else if (arg.NameEquals.Name.Identifier.Text is "False")
                            falseArg = arg;
                    }

                    if (trueArg is null && falseArg is null)
                        continue;

                    boolSpec = new(trueArg?.Expression.ToString(), falseArg?.Expression.ToString());
                }
                else if (attrFullName == KnownTypes.CoalesceToNullAttribute)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    toNull.Add(expr.ToString());
                }
                else if (attrFullName == KnownTypes.EmptyDefaultsToAttribute)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    if (ctx.SemanticModel.GetSymbolInfo(expr).Symbol is IFieldSymbol fieldSymbol)
                        fromEmpty = fieldSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                            .WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType));
                    else
                        fromEmpty = expr.ToString();
                }
                else if (attrFullName == KnownTypes.Base64EncodeAttribute)
                    transforms.Add(new Transform.Base64());
                else if (attrFullName == KnownTypes.XorAttribute)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    transforms.Add(new Transform.Xor(expr.ToString()));
                }
                else if (attrFullName == KnownTypes.GzipAttribute)
                    transforms.Add(new Transform.Gzip());
                else if (attrFullName == KnownTypes.SeparatorAttribute)
                    separator = attr.ArgumentList?.Arguments.FirstOrDefault()?.Expression.ToString();
            }

            if (index is not null)
            {
                var propName = prop.Identifier.ToString();
                var required = prop.Modifiers.Any(m => m.Kind() is SyntaxKind.RequiredKeyword);

                if (PropTypeInfo.TryCreate(prop, ctx.SemanticModel, out var propTypeInfo)
                    && ctx.SemanticModel.GetTypeInfo(prop.Type).Type is { } typeSymbol)
                {
                    var onDeserializingHooked =
                        members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m =>
                            m.Modifiers.Any(tok => tok.Kind() is SyntaxKind.PartialKeyword)
                            && m.ParameterList.Parameters is [{ }]
                            && m.Identifier.Text == $"On{prop.Identifier}Deserializing")
                        is not null;

                    infos.Add(new(
                        propTypeInfo, required, propName, index, boolSpec, transforms.ToEquatableArray(),
                        toNull.ToEquatableArray(), fromEmpty, onDeserializingHooked, separator));
                }
            }
        }

        return infos.ToEquatableArray();
    }
}
