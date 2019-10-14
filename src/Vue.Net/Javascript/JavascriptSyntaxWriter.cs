using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vue.Net.Javascript
{
    public class JavascriptSyntaxWriter : CSharpSyntaxWalker
    {
        public static Dictionary<Type, bool> IgnoredTypes => new Dictionary<Type, bool>
        {

        };

        public OldJavascriptTextWriter Writer { get; }

        public JavascriptSyntaxWriter(OldJavascriptTextWriter writer)
        {
            Writer = writer;
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            Writer.WriteStartJsFunctionDefinition();
            Writer.WriteStartJsFunctionSignature(node.Keyword.ValueText);
            switch (node.Kind())
            {
                case SyntaxKind.GetAccessorDeclaration:
                    break;
                case SyntaxKind.SetAccessorDeclaration:
                    Writer.WriteJsRawValue("value");
                    break;
                default:
                    throw new NotSupportedException($"Unknown Accessor Declaration: {node.Kind()}");
            }

            Writer.WriteEndJsFunctionSignature();

            if (null == node.Body)
            {
                Writer.WriteStartJsFunctionBlock();
                Writer.WriteEndJsFunctionBlock();
            }
            else
            {
                Visit(node.Body);
            }
            Writer.WriteEndJsFunctionDefinition();
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            Writer.WriteStartJsObject();
            foreach (var initializer in node.Initializers)
            {
                Visit(initializer);
            }
            Writer.WriteEndJsObject();
        }
        public override void VisitArgument(ArgumentSyntax node)
        {
            Visit(node.Expression);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            Visit(node.Left);
            Writer.WriteJsRawValue("=");
            Visit(node.Right);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            Writer.WriteJsRawValue("await ");
            Visit(node.Expression);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            Visit(node.Left);
            Writer.WriteJsRawValue(" ");
            Writer.WriteJsRawValue(node.OperatorToken.Text);
            Visit(node.Right);
            Writer.WriteJsRawValue(" ");
        }

        public override void VisitBlock(BlockSyntax node)
        {
            Writer.WriteStartJsFunctionBlock();
            foreach (var statement in node.Statements)
            {
                Visit(statement);
            }
            Writer.WriteEndJsFunctionBlock();
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            Visit(node.Expression);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Writer.WriteStartJsVariableStatement(node.Identifier.ValueText);

            Writer.WriteStartJsFunctionDefinition();

            Writer.WriteStartJsFunctionSignature("function");
            Writer.WriteEndJsFunctionSignature();

            Writer.WriteStartJsFunctionBlock();

            Writer.WriteStartJsReturnStatement();
            Writer.WriteStartJsObject();
            foreach (var member in node.Members)
            {
                Visit(member);
            }
            Writer.WriteEndJsObject();

            Writer.WriteEndJsStatement();
            Writer.WriteEndJsFunctionBlock();
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            foreach (var member in node.Members)
            {
                Visit(member);
            }
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            Writer.WriteLine();
            Writer.WriteJs("else ");
            Writer.WriteStartJsFunctionDefinition();
            Visit(node.Statement);
            Writer.WriteEndJsFunctionDefinition();
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            Writer.WriteJsRawValue("=");
            Visit(node.Value);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            Writer.WriteStartJsStatement();
            Visit(node.Expression);
            Writer.WriteEndJsStatement();
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            Writer.WriteJsRawValue(node.Identifier.Text);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            Writer.WriteStartJsFunctionDefinition();
            Writer.WriteStartJsIfCondition();
            Visit(node.Condition);
            Writer.WriteEndJsIfCondition();
            Visit(node.Statement);
            Visit(node.Else);
            Writer.WriteEndJsFunctionDefinition();
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            Writer.WriteJsValue(node.Token.Value);
        }
        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var declaration = node.Declaration;
            foreach(var variable in declaration.Variables)
            {
                Writer.WriteStartJsStatement("var");
                Writer.WriteJsRawValue(variable.Identifier.ValueText);
                Visit(variable.Initializer);
                Writer.WriteEndJsStatement();
            }
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            Visit(node.Expression);
            Writer.WriteJs(".");
            Writer.WriteJs(node.Name.Identifier.Text);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            Visit(node.Expression);
            Writer.WriteStartJsFunctionSignature(string.Empty);
            foreach (var argument in node.ArgumentList.Arguments)
            {
                Visit(argument);
            }
            Writer.WriteEndJsFunctionSignature();
        }

        private MethodDeclarationSyntax _currentMethod;

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var lastMethod = _currentMethod;
            _currentMethod = node;
            Writer.WriteStartJsFunctionDefinition();
            Writer.WriteStartJsFunctionSignature(node.Identifier.ValueText, node.Modifiers.Where(m=>m.ValueText == "async").Select(m=>m.ValueText).ToArray());
            foreach (var parameter in node.ParameterList.Parameters)
            {
                Visit(parameter);
            }
            Writer.WriteEndJsFunctionSignature();
            Visit(node.Body);
            Writer.WriteEndJsFunctionDefinition();
            _currentMethod = lastMethod;
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            foreach(var member in node.Members)
            {
                Visit(member);
            }
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            Writer.WriteJsRawValue(node.Identifier.ValueText);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            Writer.WriteJs(node.Keyword.Text);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            Writer.WriteJs(node.OperatorToken.Text);
            Visit(node.Operand);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            Visit(node.Operand);
            Writer.WriteJs(node.OperatorToken.Text);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Writer.WriteStartJsMember(node.Identifier.ValueText);
            Writer.WriteStartJsObject();
            foreach (var accessor in node.AccessorList.Accessors)
            {
                Visit(accessor);
            }
            Writer.WriteEndJsObject();
            Writer.WriteEndJsMember();
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            Writer.WriteStartJsReturnStatement();
            Visit(node.Expression);
            Writer.WriteEndJsStatement();
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            Writer.WriteJsRawValue("this");
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            Writer.WriteJsRawValue($"{node.Left}.{node.Right}");
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            if (!IgnoredTypes.TryGetValue(node.GetType(), out var terminating)){
                throw new NotSupportedException($"{node.GetType().Name} is not supported at: {node}");
            }

            if (!terminating)
            {
                base.DefaultVisit(node);
            }
        }
    }
}
