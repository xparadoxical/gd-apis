using System.Reflection;

using GeometryDash.Server.Serialization.Generator.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometryDash.Server.Serialization.Generator;

[Generator(LanguageNames.CSharp)]
public sealed partial class SerializerGenerator : IIncrementalGenerator
{
    public const string SeparatorAttributeType = "GeometryDash.Server.Serialization.SeparatorAttribute";
    public const string KeyedAttributeType = "GeometryDash.Server.Serialization.KeyedAttribute";

    public const string FieldAttributeType = "GeometryDash.Server.Serialization.FieldAttribute";
    public const string BoolAttributeType = "GeometryDash.Server.Serialization.BoolAttribute";

    public const string CoalesceToNullAttributeType = "GeometryDash.Server.Serialization.CoalesceToNullAttribute";
    public const string EmptyDefaultsToAttributeType = "GeometryDash.Server.Serialization.EmptyDefaultsToAttribute";

    public const string Base64EncodedAttributeType = "GeometryDash.Server.Serialization.Base64EncodedAttribute";
    public const string XorAttributeType = "GeometryDash.Server.Serialization.XorAttribute";
    public const string GzipAttributeType = "GeometryDash.Server.Serialization.GzipAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //System.Diagnostics.Debugger.Launch();
#endif

        var serializableTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
            SeparatorAttributeType,
            (syntax, ct) => syntax is ClassDeclarationSyntax,
            (ctx, ct) =>
            {
                var decl = ctx.TargetNode.Cast<ClassDeclarationSyntax>();
                return (c: GetClassInfo(ctx, decl, ct),
                        f: GetPropertyInfos(ctx, decl.Members, ct));
            })
            .Where(info => info.c is not null)
            .Select((info, ct) => new SerializableClassInfo(info.c!, info.f));

        context.RegisterSourceOutput(serializableTypes, GenerateType);

        //context.RegisterSourceOutput(serializableTypes,
        //    (spc, names) => spc.AddSource("Test.g.cs",
        //        SourceText.From(string.Join(", ", names), Encoding.UTF8)));
    }

    public static Class? GetClassInfo(GeneratorAttributeSyntaxContext ctx, ClassDeclarationSyntax decl, CancellationToken ct)
    {
        var separatorAttr = ctx.Attributes.Single(a => a.AttributeClass!.ToDisplayString() == SeparatorAttributeType);
        //TODO SingleOrDefault
        var fieldSeparator = SymbolDisplay.FormatPrimitive(
            separatorAttr.NamedArguments.Single(arg => arg.Key is "Field").Value.Value!,
            true, false);
        var listSeparator = SymbolDisplay.FormatPrimitive(
            separatorAttr.NamedArguments.Single(arg => arg.Key is "ListItem").Value.Value!,
            true, false);

        var keyed = ctx.TargetSymbol.GetAttributes()
            .SingleOrDefault(a => a.AttributeClass!.ToDisplayString() == KeyedAttributeType) is not null;

        return new(
            decl.Parent!.Cast<BaseNamespaceDeclarationSyntax>().Name.ToString(),
            decl.Identifier.ToString(),
            $"{decl.Keyword} {decl.Identifier}{decl.TypeParameterList}",
            fieldSeparator, listSeparator, keyed);
    }

    public static EquatableArray<Field> GetPropertyInfos(GeneratorAttributeSyntaxContext ctx, SyntaxList<MemberDeclarationSyntax> members, CancellationToken ct)
    {
        var infos = new List<Field>();

        foreach (var prop in members.OfType<PropertyDeclarationSyntax>())
        {
            uint? index = null;
            BoolSpec? boolSpec = null;
            var transforms = new List<Transform>();
            var toNull = new List<string>();
            string? fromEmpty = null;
            foreach (var attr in prop.AttributeLists.SelectMany(l => l.Attributes))
            {
                var type = ctx.SemanticModel.GetTypeInfo(attr, ct).Type;
                if (type is null)
                    continue;

                var attrFullName = type.ToDisplayString();
                if (attrFullName == FieldAttributeType)
                {
                    var arglist = attr.ArgumentList;
                    if (arglist is null)
                        continue;

                    if (arglist.Arguments is not [{ Expression: var expr }])
                        continue;

                    if (expr is LiteralExpressionSyntax literal
                        && uint.TryParse(literal.ToString(), out var parsed))
                        index = parsed;
                    else if (expr is IdentifierNameSyntax identifier
                        && ctx.SemanticModel.GetConstantValue(identifier, ct) is { Value: uint constValue })
                        index = constValue;
                }
                else if (attrFullName == BoolAttributeType)
                {
                    if (attr.ArgumentList is not
                        {
                            Arguments: [{ NameEquals: null, Expression: var trueExpr }, ..]
                                and { Count: 1 or 2 }
                        })
                        continue;

                    var falseArg = attr.ArgumentList.Arguments.ElementAtOrDefault(1);
                    if (falseArg is not null
                        and not { NameEquals.Name.Identifier.Text: "False" })
                        continue;

                    boolSpec = new(trueExpr.ToString(), falseArg?.Expression.ToString());
                }
                else if (attrFullName == CoalesceToNullAttributeType)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    toNull.Add(expr.ToString());
                }
                else if (attrFullName == EmptyDefaultsToAttributeType)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    fromEmpty = expr.ToString();
                }
                else if (attrFullName == Base64EncodedAttributeType)
                    transforms.Add(new Transform.Base64());
                else if (attrFullName == XorAttributeType)
                {
                    if (attr.ArgumentList is not { Arguments: [{ Expression: var expr }] })
                        continue;

                    if (ctx.SemanticModel.GetTypeInfo(expr, ct).Type is not { } typeSymbol)
                        continue;

                    transforms.Add(new Transform.Xor(expr.ToString(), typeSymbol.Name));
                }
                else if (attrFullName == GzipAttributeType)
                    transforms.Add(new Transform.Gzip());
            }

            if (index is not null)
            {
                var propName = prop.Identifier.ToString();
                var required = prop.Modifiers.Any(m => m.Kind() is SyntaxKind.RequiredKeyword);

                if (FieldTypeInfo.TryCreate(prop, ctx.SemanticModel, out var fieldTypeInfo))
                    infos.Add(new(fieldTypeInfo, required, propName, index.Value, boolSpec,
                        transforms.ToEquatableArray(), toNull.ToEquatableArray(), fromEmpty));
            }
        }

        return infos.ToEquatableArray();
    }
}
