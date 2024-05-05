using System.Reflection;
using System.Text;

using GeometryDash.Server.Serialization.Generator.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GeometryDash.Server.Serialization.Generator;

[Generator(LanguageNames.CSharp)]
public sealed partial class SerializerGenerator : IIncrementalGenerator
{
    public static readonly string SeparatorAttributeType = typeof(SeparatorAttribute).FullName;
    public static readonly string KeyedAttributeType = typeof(KeyedAttribute).FullName;

    public static readonly string FieldAttributeType = typeof(FieldAttribute).FullName;
    public static readonly string BoolAttributeType = typeof(BoolAttribute).FullName;

    public static readonly string CoalesceToNullAttributeType = typeof(CoalesceToNullAttribute).FullName;
    public static readonly string EmptyDefaultsToAttributeType = typeof(EmptyDefaultsToAttribute).FullName;

    public static readonly string Base64EncodedAttributeType = typeof(Base64EncodedAttribute).FullName;
    public static readonly string XorAttributeType = typeof(XorAttribute).FullName;
    public static readonly string GzipAttributeType = typeof(GzipAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //System.Diagnostics.Debugger.Launch();
#endif

        context.RegisterPostInitializationOutput(ctx =>
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(SerializerGenerator), "Attributes.cs");
            ctx.AddSource("Attributes.g.cs", SourceText.From(stream, Encoding.UTF8));
        });

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

        var partialDeclaration = decl.AddModifiers(SyntaxFactory.ParseToken("partial"));
        var partialDeclarator = $"{partialDeclaration.ConstraintClauses}";

        return new(
            decl.Parent!.Cast<BaseNamespaceDeclarationSyntax>().Name.ToString(),
            decl.Identifier.ToString(),
            $"partial {partialDeclaration.Keyword} {partialDeclaration.Identifier}{partialDeclaration.TypeParameterList}",
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
                        and not { NameEquals.Name.Identifier.Text: nameof(BoolAttribute.False) })
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
