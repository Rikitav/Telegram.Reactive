using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Telegram.Reactive.Generators
{
    [Generator(LanguageNames.CSharp)]
    public class ImplicitHandlerBuilderExtensionsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if ANALYZERSDEBUG //fgffd
            if (!Debugger.IsAttached)
                Debugger.Launch();
#endif
            IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> pipeline = context.SyntaxProvider
                .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform)
                .Where(declaration => declaration != null)
                .Collect();

            context.RegisterImplementationSourceOutput(pipeline, GenerateSource);
        }

        private static bool SyntaxPredicate(SyntaxNode node, CancellationToken _)
        {
            if (node is not ClassDeclarationSyntax classDeclaration)
                return false;

            return true;
        }

        private static ClassDeclarationSyntax SyntaxTransform(GeneratorSyntaxContext context, CancellationToken _)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
            if (symbol is null)
                return null!;

            if (symbol is not ITypeSymbol typeSymbol)
                return null!;

            if (!typeSymbol.IsAssignableFrom("UpdateFilterAttribute"))
                return null!;

            return (ClassDeclarationSyntax)context.Node;
        }

        private static void GenerateSource(SourceProductionContext context, ImmutableArray<ClassDeclarationSyntax> declarations)
        {
            context.AddSource("Test.cs", "// хуй");
            //return;

            List<string> usings = ["using System;", "using Telegram.Reactive.Attributes;", "using Telegram.Reactive.FilterAttributes;", "using Telegram.Reactive.Handlers.Building;", "using Telegram.Reactive.Core.Components.Handlers.Building;"];
            Dictionary<string, string> targeters = [];

            StringBuilder sourceBuilder = new StringBuilder()
                .AppendLine("namespace Telegram.Reactive")
                .AppendLine("{")
                .Append("\t//").Append(string.Join(", ", declarations.Select(decl => decl.Identifier.ToString()))).AppendLine()
                .AppendLine("\tpublic static partial class HandlerBuilderExtensions")
                .AppendLine("\t{");

            List<ClassDeclarationSyntax> lateTargeterClasses = [];
            foreach (ClassDeclarationSyntax classDeclaration in declarations)
            {
                try
                {
                    ParseClassDeclaration(sourceBuilder, classDeclaration, usings, targeters);
                }
                catch (TargteterNotFoundException)
                {
                    lateTargeterClasses.Add(classDeclaration);
                }
                catch (Exception exc)
                {
                    string errorFormat = string.Format("\t\t// failed to generate for {0} : {1}", classDeclaration.Identifier.ToString(), exc.GetType().Name);
                    sourceBuilder.AppendLine(errorFormat);
                }
            }

            foreach (ClassDeclarationSyntax classDeclaration in lateTargeterClasses)
            {
                try
                {
                    ParseClassDeclaration(sourceBuilder, classDeclaration, usings, targeters);
                }
                catch (Exception exc)
                {
                    string errorFormat = string.Format("\t\t// failed to generate for {0} : {1}", classDeclaration.Identifier.ToString(), exc.GetType().Name);
                    sourceBuilder.AppendLine(errorFormat);
                }
            }

            sourceBuilder.AppendLine("\t}\n}");
            sourceBuilder.Insert(0, string.Join("\n", usings.Select(use => use.Trim()).Distinct().OrderBy(use => use)) + "\n\n");
            context.AddSource("GeneratedHandlerBuilderExtensions.cs", sourceBuilder.ToString());
        }

        private static void ParseClassDeclaration(StringBuilder sourceBuilder, ClassDeclarationSyntax classDeclaration, List<string> usings, Dictionary<string, string> targeters)
        {
            CompilationUnitSyntax compilationUnit = classDeclaration.FindCompilationUnitSyntax();
            usings.AddRange(compilationUnit.Usings.Select(use => use.ToFullString()));
            
            IEnumerable<MethodDeclarationSyntax> methods = classDeclaration.Members.WhereCast<MethodDeclarationSyntax>();
            MethodDeclarationSyntax? targeterMethod = methods.FirstOrDefault(method => method.Identifier.ToString() == "GetFilterringTarget");

            string className = classDeclaration.Identifier.ToString();
            string filterName = className.Replace("Attribute", string.Empty);
            string classTargetterMethodName = filterName + "_GetFilterringTarget";

            if (targeterMethod != null)
            {
                targeters.Add(className, classTargetterMethodName);
                RenderTargeterMethod(sourceBuilder, classTargetterMethodName, targeterMethod);
                sourceBuilder.AppendLine();
            }
            else
            {
                if (classDeclaration.BaseList == null)
                    throw new Exception();

                string baseClassName = classDeclaration.BaseList.Types
                    .ElementAt(0).GetBaseTypeSyntaxName();

                if (!targeters.ContainsKey(baseClassName))
                    throw new TargteterNotFoundException();

                classTargetterMethodName = targeters[baseClassName];
            }

            if (classDeclaration.Modifiers.Any(keyword => keyword.ValueText == "abstract"))
                return;

            if (classDeclaration.ParameterList != null)
            {
                if (classDeclaration.BaseList != null)
                {
                    PrimaryConstructorBaseTypeSyntax primaryConstructor = (PrimaryConstructorBaseTypeSyntax)classDeclaration.BaseList.Types.ElementAt(0);
                    RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, classDeclaration.ParameterList.Parameters, primaryConstructor.ArgumentList.Arguments);
                }
                else
                {
                    RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, classDeclaration.ParameterList.Parameters, []);
                }

                sourceBuilder.AppendLine();
            }

            foreach (ConstructorDeclarationSyntax constructor in classDeclaration.Members.WhereCast<ConstructorDeclarationSyntax>())
            {
                if (constructor.Initializer == null)
                    continue;

                RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, constructor.ParameterList.Parameters, constructor.Initializer.ArgumentList.Arguments);
                sourceBuilder.AppendLine();
            }
        }

        private static void RenderExtensionMethod(StringBuilder sourceBuilder, string filterName, string classTargetterMethodName, SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
#if DEBUG
            sourceBuilder
                .AppendLine("\t\t/// <summary>")
                .AppendLine("\t\t/// Test description")
                .AppendLine("\t\t/// </summary>");
#endif
            sourceBuilder.Append("\t\tpublic static TBuilder ").Append(filterName).Append("<TBuilder>(this IHandlerBuilderActions<TBuilder> builder");

            if (parameters.Any())
                sourceBuilder.Append(", ").Append(parameters.ToFullString());

            sourceBuilder.Append(") where TBuilder : HandlerBuilderBase").AppendLine();
            sourceBuilder.Append("\t\t\t=> builder.AddTargetedFilter");

            if (arguments.Count > 1)
                sourceBuilder.Append("s");

            sourceBuilder.Append("(").Append(classTargetterMethodName);

            if (arguments.Any())
                sourceBuilder.Append(", ").Append(arguments.ToFullString());
            
            sourceBuilder.Append(");").AppendLine();
        }

        private static void RenderTargeterMethod(StringBuilder sourceBuilder, string classTargetterMethodName, MethodDeclarationSyntax targeterMethod)
        {
            sourceBuilder.Append("\t\tprivate static ").Append(targeterMethod.ReturnType.ToString()).Append(" ").Append(classTargetterMethodName).Append(targeterMethod.ParameterList.ToFullString());

            if (targeterMethod.ExpressionBody != null)
            {
                sourceBuilder.Append(targeterMethod.ExpressionBody.ToFullString()).Append(";").AppendLine();
            }
            else if (targeterMethod.Body != null)
            {
                sourceBuilder.Append(targeterMethod.Body.ToFullString());
            }
        }
    }
}
