using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using Vue.Net.Javascript.Syntax;

namespace Vue.Net
{

    // Static Member
    // Instance Member
    // Expression -> JsExpression

    public class JsRenderer
    {
/*        public bool CanHandleSymbol(ISymbol symbol)
        {
            switch(symbol.)
            return true;
        }

        public JsExpressionSyntax RenderSymbol(ISymbol symbol)
        {

        } */
    }
    public class VueSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;

        private readonly CSharpTypeHandler _typeHandler;

        public VueSyntaxRewriter(SemanticModel model, CSharpTypeHandler typeHandler = null)
        {
            _semanticModel = model;
            _typeHandler = typeHandler ?? new CSharpTypeHandler();
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            var retVal = base.VisitIdentifierName(node);
            var info = this._semanticModel.GetSymbolInfo(node);
            if (info.Symbol is IPropertySymbol)
            {
                var containingType = info.Symbol.ContainingType;
                var newExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName(node.Identifier.Text));
                retVal = retVal.ReplaceNode(retVal, newExpression);
            }

            return retVal;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node);
            var invokedSymbol = symbolInfo.Symbol as IMethodSymbol;
            SyntaxNode rewrittenNode = base.VisitInvocationExpression(node);
            if (node.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var containingType = invokedSymbol.ContainingType;
                var nameFormat =
                    new SymbolDisplayFormat(
                        SymbolDisplayGlobalNamespaceStyle.Omitted,
                        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
                    .AddGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
                    .AddMemberOptions(SymbolDisplayMemberOptions.IncludeParameters)
                    .AddParameterOptions(SymbolDisplayParameterOptions.IncludeType);

                var classType = invokedSymbol.ReceiverType as INamedTypeSymbol;
                var types = new List<INamedTypeSymbol>() { classType };
                types.AddRange(classType.Interfaces);

                if (classType.IsGenericType)
                {
                    types.Add(classType.ConstructedFrom);
                    types.AddRange(classType.ConstructedFrom.Interfaces);
                }

                foreach(var type in types)
                {
                    var methodName = $"{type.ToDisplayString()}:{invokedSymbol.OriginalDefinition.ToDisplayString(nameFormat)}";
                    var handledNode = _typeHandler.HandleMember(methodName, rewrittenNode);
                    if (null != handledNode)
                    {
                        return handledNode;
                    }
                }
                throw new ArgumentException($"Unable to handle method {node}");     
            }
            return rewrittenNode;
        }
    }
}
