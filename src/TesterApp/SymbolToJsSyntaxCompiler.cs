using Microsoft.CodeAnalysis;
using System;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{
    public class SymbolToJsSyntaxCompiler : ICompiler<ISymbol, JsSyntax>
    {
        private ICompiler<ISymbol, string> IdentityCompiler { get; }

        public SymbolToJsSyntaxCompiler(ICompiler<ISymbol, string> identityCompiler)
        {
            IdentityCompiler = identityCompiler;
        }

        public JsSyntax Compile(ISymbol symbol)
        {
            return symbol switch
            {
                ITypeSymbol _ => JsTypeExpression(IdentityCompiler.Compile(symbol)),
                IMethodSymbol methodSymbol => JsMethodExpression(IdentityCompiler.Compile(methodSymbol.ContainingType), IdentityCompiler.Compile(symbol)),
                IParameterSymbol _ => JsParameterExpression(IdentityCompiler.Compile(symbol)),
                IFieldSymbol _ => JsFieldExpression(IdentityCompiler.Compile(symbol)),
                ILocalSymbol _ => JsLiteralExpression(IdentityCompiler.Compile(symbol)),
                IPropertySymbol _ => new JsInvocationExpressionSyntax(
                    new JsMemberAccessorExpressionSyntax(
                        new JsLiteralExpressionSyntax(JsSyntaxToken.This),
                        JsSyntaxToken.Identifier(IdentityCompiler.Compile(symbol))),
                    new JsArgumentListSyntax(Array.Empty<JsExpressionSyntax>())),
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol.GetType().Name, null)
            };
        }

        public JsExpressionSyntax JsTypeExpression(string typeName) =>
            new JsElementAccessExpressionSyntax(
                new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier("globalThis")),
                new JsExpressionSyntax[]
                {
                    new JsLiteralExpressionSyntax(JsSyntaxToken.StringLiteral(typeName))
                });

        public JsExpressionSyntax JsMethodExpression(string typeName, string methodName) =>
            new JsMemberAccessorExpressionSyntax(JsTypeExpression(typeName), JsSyntaxToken.Identifier(methodName));

        public JsExpressionSyntax JsParameterExpression(string parameterName) =>
            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(parameterName));

        public JsExpressionSyntax JsFieldExpression(string fieldName) =>
            new JsMemberAccessorExpressionSyntax(
                new JsLiteralExpressionSyntax(JsSyntaxToken.This),
                JsSyntaxToken.Identifier(fieldName));

        public JsLiteralExpressionSyntax JsLiteralExpression(string literal) =>
            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(literal));
    }
}
