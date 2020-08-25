using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vue.Net.Javascript;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{

    public class SyntaxNodeToJsSyntaxCompiler : CSharpSyntaxVisitor<JsSyntax>, ICompiler<SyntaxNode, JsSyntax>
    {

        public SemanticModel Model { get; set; }

        public ICompiler<ISymbol, JsSyntax> SymbolCompiler { get; }

        public ICompiler<ISymbol, string> SymbolIdentityCompiler { get; }

        public SyntaxNodeToJsSyntaxCompiler(
            SemanticModel model,
            ICompiler<ISymbol,JsSyntax> symbolCompiler,
            ICompiler<ISymbol,string> symbolIdentityCompiler)
        {
            Model = model;
            SymbolCompiler = symbolCompiler;
            SymbolIdentityCompiler = symbolIdentityCompiler;
        }

        public JsSyntax Compile(SyntaxNode node) => Visit(node);


        public JsSyntaxToken VisitSyntaxToken(SyntaxToken syntaxToken) => JsSyntaxToken.Identifier(syntaxToken.ValueText);

        public override JsSyntax DefaultVisit(SyntaxNode node) => throw new ArgumentOutOfRangeException($"Unabled to handle NodeType: {node.GetType().Name}");

        public override JsSyntax VisitBlock(BlockSyntax node) => null == node
            ? JsBlockSyntax.Empty
            : new JsBlockSyntax(node.Statements.Select(statement => Visit(statement) as JsStatementSyntax).ToArray());

        public override JsSyntax VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) =>
            new JsSeperatedSyntaxList<JsSyntax>(
                JsSyntaxToken.NewLine,
                node.Members.Select(member => Visit(member)).ToArray());

        public override JsSyntax VisitCompilationUnit(CompilationUnitSyntax node) =>
            new JsSeperatedSyntaxList<JsSyntax>(
                JsSyntaxToken.NewLine,
                node.Members.Select(member => Visit(member)).ToArray());

        public override JsSyntax VisitBinaryExpression(BinaryExpressionSyntax node) => new JsBinaryExpressionSyntax(
            VisitSyntaxToken(node.OperatorToken),
            Visit(node.Left) as JsExpressionSyntax,
            Visit(node.Right) as JsExpressionSyntax);

        public override JsSyntax VisitExpressionStatement(ExpressionStatementSyntax node) =>
            new JsExpressionStatementSyntax(Visit(node.Expression) as JsExpressionSyntax);

        public override JsSyntax VisitIdentifierName(IdentifierNameSyntax node)
        {
            var symbol = Model.GetSymbolInfo(node).Symbol;
            return SymbolCompiler.Compile(symbol);
        }

        public override JsSyntax VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            if (node.Left is IdentifierNameSyntax idSyntax)
            {
                var symbol = Model.GetSymbolInfo(idSyntax).Symbol;
                switch (symbol.Kind)
                {
                    case SymbolKind.Property: return new JsInvocationExpressionSyntax(
                        new JsMemberAccessorExpressionSyntax(
                            new JsLiteralExpressionSyntax(JsSyntaxToken.This),
                            JsSyntaxToken.Identifier(GetMethodName(MethodKind.PropertySet, idSyntax.Identifier.ValueText, Array.Empty<string>()))),
                        new JsArgumentListSyntax(new JsExpressionSyntax[] { Visit(node.Right) as JsExpressionSyntax }));
                }
            }
            return new JsAssignmentExpressionSyntax(
                JsSyntaxToken.EqualSign,
                Visit(node.Left) as JsExpressionSyntax ?? throw new ArgumentOutOfRangeException($"Couldn't convert to Expression Syntax: {node.Left.GetType().Name}"),
                Visit(node.Right) as JsExpressionSyntax) ?? throw new ArgumentOutOfRangeException($"Couldn't convert to Expression Syntax: {node.Right.GetType().Name}");
        }

        public override JsSyntax VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {

            return base.VisitElementAccessExpression(node);
        }
        public override JsSyntax VisitReturnStatement(ReturnStatementSyntax node) =>
            new JsReturnStatementSyntax(Visit(node.Expression) as JsExpressionSyntax);

        public string GetMethodPrefix(MethodKind kind) => kind switch
        {
            MethodKind.Constructor => "ctr",
            MethodKind.Ordinary => "fn",
            MethodKind.PropertyGet => "get",
            MethodKind.PropertySet => "set",
            _ => "unknown"
        };

        public string GetMethodName(MethodKind kind, string identifier, string[] parameterTypes) => $"${GetMethodPrefix(kind)}${identifier}{string.Join("$", parameterTypes)}$";

        public string GetFieldName(string fieldName) => $"$data${fieldName}$";


        public override JsSyntax VisitQualifiedName(QualifiedNameSyntax node)
        {
            return new JsMemberAccessorExpressionSyntax(
                Visit(node.Right) as JsExpressionSyntax,
                Visit(node.Left) as JsSyntaxToken);
        }

        public string VisitNameSyntax(NameSyntax node) => node switch
        {
            QualifiedNameSyntax qName => $"{GetTypeName(qName.Left)}.{qName.Right.Identifier.ValueText}",
            SimpleNameSyntax sName => sName.Identifier.ValueText,
            AliasQualifiedNameSyntax aName => aName.Alias.Identifier.ValueText,
            _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };

        public string VisitTypeSyntax(TypeSyntax node) => node switch
        {
            NameSyntax name => VisitNameSyntax(name),
            PredefinedTypeSyntax preSyntax => preSyntax.Keyword.ValueText,
            _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };

        public string GetTypeName(SyntaxNode node)
        {
            var sb = new StringBuilder();
            if (null == node) return string.Empty;
            switch(node)
            {
                case CompilationUnitSyntax _: return string.Empty;
                case ClassDeclarationSyntax cls: 
                    var parentName = GetTypeName(node.Parent);
                    return parentName == null ? cls.Identifier.ValueText : $"{parentName}.{cls.Identifier.ValueText}";
                case NamespaceDeclarationSyntax ns:
                    return VisitNameSyntax(ns.Name);
                case IdentifierNameSyntax id:
                    var symbol = Model.GetSymbolInfo(id).Symbol;
                    var idName = SymbolIdentityCompiler.Compile(symbol);
                    return idName;
                default: throw new ArgumentOutOfRangeException(nameof(node), node, null);
            };
        }

        public override JsSyntax VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var symbol = Model.GetSymbolInfo(node.Parent).Symbol;
            if (null == symbol)
            {
                symbol = Model.GetSymbolInfo(node).Symbol;
                if (symbol is IPropertySymbol propSymbol)
                {
                    return new JsInvocationExpressionSyntax(
                        new JsMemberAccessorExpressionSyntax(
                            Visit(node.Expression) as JsExpressionSyntax,
                            JsSyntaxToken.Identifier(GetMethodName(MethodKind.PropertyGet, node.Name.Identifier.ValueText, Array.Empty<string>()))),
                        new JsArgumentListSyntax(Array.Empty<JsExpressionSyntax>()));
                }
            }
            return new JsMemberAccessorExpressionSyntax(
                Visit(node.Expression) as JsExpressionSyntax,
                JsSyntaxToken.Identifier(SymbolIdentityCompiler.Compile(symbol)));
        }

        public override JsSyntax VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            return null == node.Initializer
                ? new JsArrayCreationExpressionSyntax(Array.Empty<JsExpressionSyntax>())
                : new JsArrayCreationExpressionSyntax(node.Initializer.Expressions.Select(expression=>Visit(expression) as JsExpressionSyntax).ToArray());
        }

        public JsExpressionSyntax JsGetTypeExpression(string typeName) =>
            new JsParenthesizedExpressionSyntax(JsTypeExpression(typeName));

        public JsExpressionSyntax JsTypeExpression(string typeName) =>
            new JsElementAccessExpressionSyntax(
                new JsLiteralExpressionSyntax(JsSyntaxToken.GlobalThis),
                new JsExpressionSyntax[]
                {
                    new JsLiteralExpressionSyntax(JsSyntaxToken.StringLiteral(typeName))
                });

        public JsExpressionSyntax JsMethodExpression(string typeName, string methodName) =>
            new JsMemberAccessorExpressionSyntax(JsTypeExpression(typeName), JsSyntaxToken.Identifier(methodName));

        public JsExpressionSyntax JsParameterExpression(string parameterName) =>
            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(parameterName));

        public JsExpressionSyntax JsLocalExpression(string localName) =>
            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(localName));

        public JsExpressionSyntax JsFieldExpression(string fieldName) =>
            new JsMemberAccessorExpressionSyntax(
                new JsLiteralExpressionSyntax(JsSyntaxToken.This),
                JsSyntaxToken.Identifier(fieldName));

        public JsLiteralExpressionSyntax JsLiteralExpression(string literal) =>
            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(literal));

        public override JsSyntax VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var members = node.Members.Select(member => Visit(member)).ToList();
            var constructorToken = JsSyntaxToken.Identifier("constructor");
            var argumentsToken = JsSyntaxToken.Identifier("args");

            members.Add(new JsMethodPropertySyntax(
                JsSyntaxToken.Identifier("constructor"),
                new JsSyntaxToken[] { constructorToken, argumentsToken },
                new JsBlockSyntax(new JsStatementSyntax[]
                {
                    new JsExpressionStatementSyntax(
                        new JsInvocationExpressionSyntax(
                            new JsMemberAccessorExpressionSyntax(
                                new JsElementAccessExpressionSyntax(
                                    new JsLiteralExpressionSyntax(JsSyntaxToken.This),
                                    new JsExpressionSyntax[]{ new JsLiteralExpressionSyntax(constructorToken) }),
                                JsSyntaxToken.Identifier("apply")),
                            new JsArgumentListSyntax(new JsExpressionSyntax[] { new JsLiteralExpressionSyntax(JsSyntaxToken.This), new JsLiteralExpressionSyntax(argumentsToken) })
                            ))
                }),
                Array.Empty<JsSyntaxToken>()));

            var typeName = GetTypeName(node);
            return new JsExpressionStatementSyntax(new JsAssignmentExpressionSyntax(
                JsSyntaxToken.EqualSign,
                JsTypeExpression(typeName),
                new JsClassExpressionSyntax(members.ToArray())));
        }

        public override JsSyntax VisitArgumentList(ArgumentListSyntax node) =>
            new JsArgumentListSyntax(node.Arguments.Select(argument => Visit(argument.Expression) as JsExpressionSyntax).ToArray());

        public override JsSyntax VisitInvocationExpression(InvocationExpressionSyntax node) => new JsInvocationExpressionSyntax(
                Visit(node.Expression) as JsExpressionSyntax,
                Visit(node.ArgumentList) as JsArgumentListSyntax);

        public override JsSyntax VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var autoFieldName = GetFieldName($"auto${node.Identifier}");
            var autoField = JsFieldExpression(autoFieldName);

            var statements = new List<JsSyntax>();
            if (null != node.ExpressionBody)
            {
                statements.Add(new JsMethodPropertySyntax(
                    JsSyntaxToken.Identifier(GetMethodName(MethodKind.PropertyGet, node.Identifier.ValueText, Array.Empty<string>())),
                    Array.Empty<JsSyntaxToken>(),
                    new JsBlockSyntax(new JsStatementSyntax[] { new JsReturnStatementSyntax(Visit(node.ExpressionBody.Expression) as JsExpressionSyntax) }),
                    Array.Empty<JsSyntaxToken>()));
            }
            else if (!node.AccessorList.Accessors.Any(accessor=> null != accessor.Body || null != accessor.ExpressionBody))
            {
                statements.Add(
                    new JsExpressionStatementSyntax(
                        new JsAssignmentExpressionSyntax(
                            JsSyntaxToken.EqualSign,
                            new JsLiteralExpressionSyntax(JsSyntaxToken.Identifier(autoFieldName)),
                            node.ExpressionBody != null ? Visit(node.ExpressionBody.Expression) as JsExpressionSyntax : null == node.Initializer ? JsExpressionSyntax.Null : Visit(node.Initializer.Value) as JsExpressionSyntax)));
            }

            if (null != node.AccessorList)
            {
                statements.AddRange(node.AccessorList.Accessors.Select(accessor => accessor.Kind() switch
                {
                    SyntaxKind.GetAccessorDeclaration => new JsMethodPropertySyntax(
                        JsSyntaxToken.Identifier(GetMethodName(MethodKind.PropertyGet, node.Identifier.ValueText, Array.Empty<string>())),
                        Array.Empty<JsSyntaxToken>(),
                        null == accessor.Body ? new JsBlockSyntax(new JsStatementSyntax[] { null == accessor.ExpressionBody ? new JsReturnStatementSyntax(autoField) : new JsReturnStatementSyntax(Visit(accessor.ExpressionBody.Expression) as JsExpressionSyntax) }) : Visit(accessor.Body) as JsBlockSyntax,
                        Array.Empty<JsSyntaxToken>()),
                    SyntaxKind.SetAccessorDeclaration => new JsMethodPropertySyntax(
                        JsSyntaxToken.Identifier(GetMethodName(MethodKind.PropertySet, node.Identifier.ValueText, Array.Empty<string>())),
                        new JsSyntaxToken[1] { JsSyntaxToken.ValueParamterName },
                            null == accessor.Body
                            ? new JsBlockSyntax(new JsStatementSyntax[]
                            {
                            new JsExpressionStatementSyntax(
                                new JsAssignmentExpressionSyntax(
                                    JsSyntaxToken.EqualSign,
                                    autoField,
                                    new JsLiteralExpressionSyntax(JsSyntaxToken.ValueParamterName)))
                            })
                            : Visit(accessor.Body) as JsBlockSyntax,
                            Array.Empty<JsSyntaxToken>()
                            ),
                    _ => throw new ArgumentOutOfRangeException(nameof(accessor), accessor, null)
                }).ToArray());
            }

            return new JsSeperatedSyntaxList<JsSyntax>(
                null,
                statements.ToArray());
        }

        public override JsSyntax VisitEqualsValueClause(EqualsValueClauseSyntax node) => new JsEqualsValueSyntax(Visit(node.Value) as JsExpressionSyntax);

        public override JsSyntax VisitLiteralExpression(LiteralExpressionSyntax node) =>
            new JsLiteralExpressionSyntax(node.Kind() switch
            {
                SyntaxKind.StringLiteralExpression => JsSyntaxToken.StringLiteral(node.Token.ValueText),
                SyntaxKind.CharacterLiteralExpression => JsSyntaxToken.StringLiteral(node.Token.ValueText),
                SyntaxKind.NullLiteralExpression => JsSyntaxToken.Null,
                SyntaxKind.FalseLiteralExpression => JsSyntaxToken.Identifier(node.Token.ValueText),
                SyntaxKind.NumericLiteralExpression => JsSyntaxToken.NumberLiteral(node.Token.ValueText),
                _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
            });


        public override JsSyntax VisitVariableDeclarator(VariableDeclaratorSyntax node) => new JsVariableDeclarationStatementSyntax(
            new JsIdentifierNameSyntax(JsSyntaxToken.Identifier(node.Identifier.ValueText)),
            Visit(node.Initializer) as JsExpressionSyntax);

        public override JsSyntax VisitVariableDeclaration(VariableDeclarationSyntax node) => new JsStatementListSyntax(node.Variables.Select(variable => Visit(variable) as JsStatementSyntax).ToArray());

        public override JsSyntax VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) => Visit(node.Declaration);

        public override JsSyntax VisitArgument(ArgumentSyntax node) => Visit(node.Expression);

        public override JsSyntax VisitParameter(ParameterSyntax node)
        {
            return new JsIdentifierNameSyntax(JsSyntaxToken.Identifier(node.Identifier.ValueText));
        }

        private string[] GetParameterTypes(ParameterListSyntax parameterList) => parameterList.Parameters.Select(parameter => Model.GetTypeInfo(parameter.Type).Type.ToDisplayString()).ToArray();

        public override JsSyntax VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var symbolInfo = Model.GetSymbolInfo(node);
            var type = (symbolInfo.Symbol as IMethodSymbol).ContainingType;
            var typeName = type.ToDisplayString();
            var temp = new JsNewExpressionSyntax(
                new JsInvocationExpressionSyntax(
                    new JsParenthesizedExpressionSyntax(JsTypeExpression(typeName)),
                    new JsArgumentListSyntax(
                        new JsExpressionSyntax[] {
                            new JsLiteralExpressionSyntax(JsSyntaxToken.StringLiteral(GetMethodName(MethodKind.Constructor, type.Name, type.TypeArguments.Select(ta=>ta.ContainingType.Name).ToArray()))),
                            new JsArrayCreationExpressionSyntax(
                                new JsArgumentListSyntax(node.ArgumentList?.Arguments.Select(argument=>Visit(argument.Expression) as JsExpressionSyntax).ToArray() ?? Array.Empty<JsExpressionSyntax>()))
                        })));
            return temp;
        }

        public override JsSyntax VisitMethodDeclaration(MethodDeclarationSyntax node) => new JsMethodPropertySyntax(
            JsSyntaxToken.Identifier(GetMethodName(MethodKind.Ordinary, node.Identifier.ValueText, GetParameterTypes(node.ParameterList))),
            node.ParameterList.Parameters.Select(parameter => JsSyntaxToken.Identifier(parameter.Identifier.ValueText)).ToArray(),
            VisitBlock(node.Body) as JsBlockSyntax,
            // TODO: Support Modifiers that translate
            Array.Empty<JsSyntaxToken>());

        public override JsSyntax VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => new JsMethodPropertySyntax(
            JsSyntaxToken.Identifier(GetMethodName(MethodKind.Constructor, node.Identifier.ValueText, node.ParameterList.Parameters.Select(parameter=> VisitTypeSyntax(parameter.Type)).ToArray())),
            node.ParameterList.Parameters.Select(parameter => JsSyntaxToken.Identifier(parameter.Identifier.ValueText)).ToArray(),
            VisitBlock(node.Body) as JsBlockSyntax,
            Array.Empty<JsSyntaxToken>());

        public override JsSyntax VisitFieldDeclaration(FieldDeclarationSyntax node) =>
            new JsSeperatedSyntaxList<JsStatementSyntax>(
                JsSyntaxToken.NewLine,
                node.Declaration.Variables.Select(
                    variable => new JsExpressionStatementSyntax(
                        null == variable.Initializer
                        ? JsLiteralExpression(GetFieldName(variable.Identifier.ValueText)) as JsExpressionSyntax
                        : new JsAssignmentExpressionSyntax(
                            JsSyntaxToken.EqualSign,
                            JsLiteralExpression(GetFieldName(variable.Identifier.ValueText)),
                            Visit(variable.Initializer.Value) as JsExpressionSyntax))).ToArray());
    }
}
