using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{

    public static class JsCSharpExtensions
    {
        public static ICompiler<SemanticModel, JsSyntax> DefaultSemanticToJsSyntaxCompiler { get; } =
            new InlineCompiler<SemanticModel,JsSyntax>((model) => model.GetJsSyntax(model.SyntaxTree.GetRoot()));

        public static ICompiler<Type, SemanticModel> DefaultTypeToSemanticModelCompiler { get; } =
            new TypeToSemanticModelCompiler();

        public static ICompiler<ISymbol,string> DefaultSymbolIdentityCompiler { get; } =
            new SymbolToStringIdentityCompiler();

        public static ICompiler<ISymbol,JsSyntax> DefaultSymbolToJsSyntaxCompiler { get; } =
            new SymbolToJsSyntaxCompiler(DefaultSymbolIdentityCompiler);

        private static ConditionalWeakTable<SemanticModel, SyntaxNodeToJsSyntaxCompiler> converters =
            new ConditionalWeakTable<SemanticModel, SyntaxNodeToJsSyntaxCompiler>();

        public static T GetJsSyntax<T>(this SemanticModel semanticModel, SyntaxNode node) where T : JsSyntax {

            if (converters.TryGetValue(semanticModel, out var compiler)) return compiler.Compile(node) as T;

            var converter = converters.GetOrCreateValue(semanticModel);
            converter.Model = semanticModel;
            return converter.Visit(node) as T;
        }

        public static JsSyntax GetJsSyntax(
            this SemanticModel semanticModel,
            SyntaxNode node,
            ICompiler<ISymbol, JsSyntax> symbolCompiler = null,
            ICompiler<ISymbol, string> symbolIdentityCompiler = null)
        {
            if (!converters.TryGetValue(semanticModel, out var compiler))
            {
                compiler = new SyntaxNodeToJsSyntaxCompiler(
                    semanticModel,
                    symbolCompiler ?? DefaultSymbolToJsSyntaxCompiler,
                    symbolIdentityCompiler ?? DefaultSymbolIdentityCompiler);
                converters.AddOrUpdate(semanticModel, compiler);
            }
            return compiler.Compile(node);
        }

        public static JavascriptRuntime AddSyntax(this JavascriptRuntime runtime, JsSyntax syntax) =>
            runtime.Syntax is JsEmptySyntax
            ? new JavascriptRuntime(syntax)
            : new JavascriptRuntime(new JsSeperatedSyntaxList<JsSyntax>(JsSyntaxToken.NewLine, runtime.Syntax, syntax));

        public static JavascriptRuntime Add<T>(this JavascriptRuntime runtime, ICompiler<T, JsSyntax> compiler, T input) => runtime.AddSyntax(compiler.Compile(input));

        public static JavascriptRuntime AddType<T>(this JavascriptRuntime runtime, ICompiler<Type, JsSyntax> compiler) => runtime.Add(compiler, typeof(T));
        public static JavascriptRuntime AddType<T>(this JavascriptRuntime runtime,
            ICompiler<Type, SemanticModel> typeDecompiler = null,
            ICompiler<SemanticModel,JsSyntax> jsCompiler = null) => runtime.AddType<T>(
            new CompilerPassthrough<Type, SemanticModel, JsSyntax>(
                typeDecompiler ?? DefaultTypeToSemanticModelCompiler,
                jsCompiler ?? DefaultSemanticToJsSyntaxCompiler));

    }
}
