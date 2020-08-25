using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Vue.Net.Javascript.Syntax;

namespace Vue.Net.Javascript
{
    public class CSharpToJavascriptTranspiler
    {
        private SemanticModel _semanticModel;
        public CSharpToJavascriptTranspiler(SemanticModel semanticModel) {
            _semanticModel = semanticModel;
        }

        public JsObjectCreationExpressionSyntax ToJsObjectCreationExpression(ClassDeclarationSyntax csharpClassSyntax)
        {
            var properties = csharpClassSyntax.Members
                .SelectMany(member =>
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax method:
                            return new JsBaseObjectPropertySyntax[] { ToJsObjectProperty(method) };
                        case FieldDeclarationSyntax field:
                            return new JsBaseObjectPropertySyntax[] { ToJsObjectProperty(field) };
                        case PropertyDeclarationSyntax property:
                            return ToBaseObjectProperties(property);
                        case IndexerDeclarationSyntax indexerDeclaration:
                        case StructDeclarationSyntax structDeclaration:
                        case EnumDeclarationSyntax enumDeclaration:
                            return Array.Empty<JsBaseObjectPropertySyntax>();
                        case ConstructorDeclarationSyntax constructorDeclaration:
                            Console.WriteLine("FIXME: Can't convert constructors");
                            return Array.Empty<JsBaseObjectPropertySyntax>();
                        default:
                            throw new NotSupportedException($"Member {member} is not supported");
                    }
                }).ToArray();

            return new JsObjectCreationExpressionSyntax(properties);
        }

        public JsObjectPropertySyntax ToJsObjectProperty(FieldDeclarationSyntax fieldSyntax)
        {
            var variable = fieldSyntax.Declaration.Variables[0];
            var identifier = ToJsToken(variable.Identifier);
            var value = null != variable.Initializer? ToJsExpression(variable.Initializer.Value) : JsExpressionSyntax.Null;
            return new JsObjectPropertySyntax(identifier, value);
        }

        public JsObjectPropertySyntax ToJsObjectProperty(MethodDeclarationSyntax methodSyntax)
        {
            var identifier = ToJsToken(methodSyntax.Identifier);
            var parameters = methodSyntax.ParameterList.Parameters.Select(p =>  new JsIdentifierNameSyntax(ToJsToken(p.Identifier))).ToArray();
            var body = null != methodSyntax.Body ? ToJsBlock(methodSyntax.Body) : null;
            return new JsObjectPropertySyntax(
                identifier,
                new JsFunctionExpressionSyntax(parameters, body));
        }

        public JsMethodPropertySyntax ToJsMethodProperty(AccessorDeclarationSyntax accessor, JsSyntaxToken identifier)
        {
            var kind = accessor.Kind();
            switch (kind)
            {
                case SyntaxKind.GetAccessorDeclaration:
                    return new JsMethodPropertySyntax(
                        identifier,
                        Array.Empty<JsSyntaxToken>(),
                        ToJsBlock(accessor.Body),
                        new[] { JsSyntaxToken.Get });
                case SyntaxKind.SetAccessorDeclaration:
                    return new JsMethodPropertySyntax(
                        identifier,
                        new JsSyntaxToken[1] {
                                JsSyntaxToken.Identifier("value")
                        },
                        ToJsBlock(accessor.Body),
                        new[] { JsSyntaxToken.Set }
                        );
                default:
                    throw new NotSupportedException($"Accessor {kind} is not supported");
            }
        }

        private JsExpressionSyntax ToDefaultValueJsExpression(PredefinedTypeSyntax predefinedSyntax)
        {
            switch (predefinedSyntax.Keyword.Kind())
            {
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.UShortKeyword:
                    return new JsLiteralExpressionSyntax(JsSyntaxToken.NumberLiteral(0));
                case SyntaxKind.CharKeyword:
                    return new JsLiteralExpressionSyntax(JsSyntaxToken.StringLiteral("\0"));
                default:
                    return JsExpressionSyntax.Null;
            }
        }

        public JsBaseObjectPropertySyntax[] ToBaseObjectProperties(PropertyDeclarationSyntax propertySyntax)
        {
            var identifier = JsSyntaxToken.Identifier(propertySyntax.Identifier.ValueText);
            if (!propertySyntax.AccessorList.Accessors.Any(accessor => null != accessor.Body))
            {
                JsExpressionSyntax value;
                if (null != propertySyntax.Initializer)
                {
                    value = ToJsExpression(propertySyntax.Initializer.Value);
                }
                else
                {
                    switch (propertySyntax.Type)
                    {
                        case ArrayTypeSyntax arraySyntax:
                            value = new JsArrayCreationExpressionSyntax(Array.Empty<JsExpressionSyntax>());
                            break;
                        case PredefinedTypeSyntax predefinedSyntax:
                            value = ToDefaultValueJsExpression(predefinedSyntax);
                            break;
                        case SimpleNameSyntax simpleNameSyntax:
                        case RefTypeSyntax refSyntax:
                        case NullableTypeSyntax nullableSyntax:
                            value = JsExpressionSyntax.Null;
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(PropertyDeclarationSyntax)} Type not recognized: {propertySyntax.Type}");

                    }
                }
                return new[]
                {
                    new JsObjectPropertySyntax(identifier, value)
                };
            }

            return propertySyntax.AccessorList.Accessors.Select(accessor => ToJsMethodProperty(accessor, identifier))
                .ToArray();
        }

        public JsBlockSyntax ToJsBlock(BlockSyntax blockSyntax)
        {
            var statements = blockSyntax.Statements.Select(statement => ToJsStatement(statement)).ToArray();
            return new JsBlockSyntax(statements);
        }

        public JsStatementSyntax ToJsStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case BlockSyntax block:
                    return new JsBlockSyntax(block.Statements.Select(s => ToJsStatement(s)).ToArray());
                case ExpressionStatementSyntax expressionSyntax:
                    return new JsExpressionStatementSyntax(ToJsExpression(expressionSyntax.Expression));
                case IfStatementSyntax ifStatement:
                    return new JsIfStatementSyntax(
                        ToJsExpression(ifStatement.Condition),
                        ToJsStatement(ifStatement.Statement),
                        new JsElseClauseSyntax(
                            null == ifStatement.Else ?
                            null : ToJsStatement(ifStatement.Else.Statement)));
                case LocalDeclarationStatementSyntax localDeclaration:
                    return ToJsVariableDeclaration(localDeclaration);
                case ThrowStatementSyntax throwStatement:
                    return new JsThrowStatementSyntax(ToJsExpression(throwStatement.Expression));
                case ReturnStatementSyntax returnStatement:
                    return new JsReturnStatementSyntax(ToJsExpression(returnStatement.Expression));
                default:
                    throw new JsTranspilationException<StatementSyntax, JsStatementSyntax>(statement);
            }
        }

        public JsVariableDeclarationStatementSyntax ToJsVariableDeclaration(LocalDeclarationStatementSyntax localSyntax)
        {
            // FIXME: We need to handle mutliple variables per declaration
            var variable = localSyntax.Declaration.Variables[0];

            return new JsVariableDeclarationStatementSyntax(
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier(variable.Identifier.ValueText)),
                ToJsExpression(variable.Initializer.Value));
        }

        public JsIdentifierNameSyntax[] ToJsIdentifierNames(ParameterListSyntax parameters)
        {
            if (null == parameters)
                return Array.Empty<JsIdentifierNameSyntax>();
            return parameters.Parameters.Select(p => new JsIdentifierNameSyntax(ToJsToken(p.Identifier))).ToArray();
        }

        public JsFunctionExpressionSyntax ToJsFunctionExpression(AnonymousMethodExpressionSyntax syntax)
        {
            return new JsFunctionExpressionSyntax(
                ToJsIdentifierNames(syntax.ParameterList),
                ToJsBlock(syntax.Block));
        }

        public JsArrowFunctionExpressionSyntax ToJsArrowFunctionExpression(ParenthesizedLambdaExpressionSyntax syntax)
        {
            return new JsArrowFunctionExpressionSyntax(
                ToJsIdentifierNames(syntax.ParameterList),
                ToJs(syntax.Body));
        }

        public JsArrowFunctionExpressionSyntax ToJsArrowFunctionExpression(SimpleLambdaExpressionSyntax syntax)
        {
            return new JsArrowFunctionExpressionSyntax(
                Array.Empty<JsIdentifierNameSyntax>(),
                ToJs(syntax.Body));
        }

        public JsObjectPropertySyntax ToJsObjectProperty(AnonymousObjectMemberDeclaratorSyntax syntax)
        {
            return new JsObjectPropertySyntax(
                ToJsToken(syntax.NameEquals.Name),
                ToJsExpression(syntax.Expression));
        }

        public JsObjectCreationExpressionSyntax ToJsObjectCreationExpression(AnonymousObjectCreationExpressionSyntax syntax)
        {
            return new JsObjectCreationExpressionSyntax(
                syntax.Initializers.Select(i => ToJsObjectProperty(i)).ToArray());
        }

        public JsArrayCreationExpressionSyntax ToJsArrayCreationExpression(ArrayCreationExpressionSyntax syntax)
        {
            return new JsArrayCreationExpressionSyntax(ToJsExpressions(syntax.Initializer));
        }

        public JsAssignmentExpressionSyntax ToJsAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            return new JsAssignmentExpressionSyntax(
                JsSyntaxToken.Identifier(syntax.OperatorToken.ValueText),
                ToJsExpression(syntax.Left),
                ToJsExpression(syntax.Right));
        }

        public JsAwaitExpressionSyntax ToJsAwaitExpression(AwaitExpressionSyntax syntax)
        {
            return new JsAwaitExpressionSyntax(ToJsExpression(syntax.Expression));
        }

        public JsBinaryExpressionSyntax ToJsBinaryExpression(BinaryExpressionSyntax syntax)
        {
            return new JsBinaryExpressionSyntax(
                ToJsToken(syntax.OperatorToken),
                ToJsExpression(syntax.Left),
                ToJsExpression(syntax.Right));
        }

        public JsExpressionSyntax ToJsExpression(CastExpressionSyntax syntax)
        {
            return ToJsExpression(syntax.Expression);
        }

        public JsExpressionSyntax ToJsExpression(DefaultExpressionSyntax syntax)
        {
            if (syntax.Type is PredefinedTypeSyntax predefinedSyntax)
                return ToDefaultValueJsExpression(predefinedSyntax);
            return JsExpressionSyntax.Null;
        }

        public JsArrayCreationExpressionSyntax ToJsArrayCreationExpression(ImplicitArrayCreationExpressionSyntax syntax)
        {
            return new JsArrayCreationExpressionSyntax(ToJsExpressions(syntax.Initializer));
        }

        public JsArrayCreationExpressionSyntax ToJsArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax syntax)
        {
            return new JsArrayCreationExpressionSyntax(ToJsExpressions(syntax.Initializer));
        }

        public JsArrayCreationExpressionSyntax ToJsArrayCreationExpression(StackAllocArrayCreationExpressionSyntax syntax)
        {
            return new JsArrayCreationExpressionSyntax(ToJsExpressions(syntax.Initializer));
        }

        public JsExpressionSyntax[] ToJsExpressions(InitializerExpressionSyntax syntax)
        {
            return syntax.Expressions.Select(expression => ToJsExpression(expression)).ToArray();
        }

        public JsExpressionSyntax ToJsExpression(ThisExpressionSyntax syntax)
        {
            return new JsLiteralExpressionSyntax(JsSyntaxToken.This);
        }

        public JsParenthesizedExpressionSyntax ToJsParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return new JsParenthesizedExpressionSyntax(ToJsExpression(syntax.Expression));
        }

        public JsExpressionSyntax ToJsExpression(ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case AnonymousMethodExpressionSyntax anonymousMethodSyntax:
                    return ToJsFunctionExpression(anonymousMethodSyntax);
                case ArrayCreationExpressionSyntax arrayCreationSyntax:
                    return ToJsArrayCreationExpression(arrayCreationSyntax);
                case AssignmentExpressionSyntax assignmentSyntax:
                    return ToJsAssignmentExpression(assignmentSyntax);
                case AwaitExpressionSyntax awaitSyntax:
                    return ToJsAwaitExpression(awaitSyntax);
                case BinaryExpressionSyntax binaryExpression:
                    return ToJsBinaryExpression(binaryExpression);
                case CastExpressionSyntax castSyntax:
                    return ToJsExpression(castSyntax);
                case DefaultExpressionSyntax defaultSyntax:
                    return ToJsExpression(defaultSyntax);
                case ImplicitArrayCreationExpressionSyntax implicitArraySyntax:
                    return ToJsArrayCreationExpression(implicitArraySyntax);
                case LiteralExpressionSyntax literal:
                    return new JsLiteralExpressionSyntax(ToJsToken(literal.Token));
                case IdentifierNameSyntax identifierExpression:
                    return new JsIdentifierNameSyntax(ToJsToken(identifierExpression.Identifier));
                case ObjectCreationExpressionSyntax createdSyntax:
                    return ToJsObjectConstructionExpression(createdSyntax);
                case InvocationExpressionSyntax invocationSyntax:
                    return new JsInvocationExpressionSyntax(
                        ToJsExpression(invocationSyntax.Expression),
                        new JsArgumentListSyntax(invocationSyntax.ArgumentList.Arguments.Select(arg => ToJsExpression(arg.Expression)).ToArray()));
                case MemberAccessExpressionSyntax memberSyntax:
                    return new JsMemberAccessorExpressionSyntax(
                        ToJsExpression(memberSyntax.Expression),
                        ToJsToken(memberSyntax.Name.Identifier));
                case PredefinedTypeSyntax predefinedSyntax:
                    return new JsIdentifierNameSyntax(ToJsToken(predefinedSyntax));
                case StackAllocArrayCreationExpressionSyntax stackAllocArrayCreationSyntax:
                    return ToJsArrayCreationExpression(stackAllocArrayCreationSyntax);
                case ThisExpressionSyntax thisExpressionSyntax:
                    return ToJsExpression(thisExpressionSyntax);
                case ParenthesizedExpressionSyntax parenthesizedExpressionSyntax:
                    return ToJsParenthesizedExpression(parenthesizedExpressionSyntax);
                case CheckedExpressionSyntax checkedSyntax:
                case ConditionalAccessExpressionSyntax conditionalAccessSyntax:
                case ConditionalExpressionSyntax conditionalSyntax:
                case DeclarationExpressionSyntax declarationSyntax:
                case ElementAccessExpressionSyntax elementAccessSyntax:
                case ImplicitElementAccessSyntax implicitElementAccessSyntax:
                case InstanceExpressionSyntax instanceSyntax:
                default:
                    throw new JsTranspilationException<ExpressionSyntax, JsExpressionSyntax>(syntax);
            }
        }

        public JsObjectConstructionExpressionSyntax ToJsObjectConstructionExpression(ObjectCreationExpressionSyntax objectExpression)
        {
            return new JsObjectConstructionExpressionSyntax(
                ToJsToken(objectExpression.Type),
                objectExpression.ArgumentList.Arguments.Select(arg => ToJsExpression(arg.Expression)).ToArray());
        }

        public JsSyntaxToken ToJsToken(TypeSyntax typeSyntax)
        {
            switch (typeSyntax)
            {
                case IdentifierNameSyntax identifier:
                    return JsSyntaxToken.Identifier(identifier.Identifier.ValueText);
                case PredefinedTypeSyntax predefinedSyntax:
                    return JsSyntaxToken.Identifier(predefinedSyntax.Keyword.ValueText);
                default:
                    throw new JsTranspilationException<TypeSyntax, JsSyntaxToken>(typeSyntax);
            }
        }

        public static JsSyntaxToken ToJsToken(SyntaxToken token)
        {
            var kind = token.Kind();
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                    return JsSyntaxToken.Identifier(token.ValueText);
                case SyntaxKind.NullKeyword:
                    return JsSyntaxToken.Null;
                case SyntaxKind.StringLiteralToken:
                    return JsSyntaxToken.StringLiteral(token.ValueText);
                case SyntaxKind.NumericLiteralToken:
                    return JsSyntaxToken.NumberLiteral(token.ValueText);
                case SyntaxKind.EqualsEqualsToken:
                    return JsSyntaxToken.EqualsEquals;
                case SyntaxKind.AmpersandAmpersandToken:
                    return JsSyntaxToken.AmpersandAmpersand;
                case SyntaxKind.ExclamationEqualsToken:
                    return JsSyntaxToken.ExclamationEquals;
                case SyntaxKind.BarBarToken:
                    return JsSyntaxToken.BarBar;
                case SyntaxKind.LessThanEqualsToken:
                    return JsSyntaxToken.LessThanEqual;
                case SyntaxKind.GreaterThanEqualsToken:
                    return JsSyntaxToken.GreaterThanEqual;
                case SyntaxKind.GreaterThanToken:
                    return JsSyntaxToken.GreaterThan;
                case SyntaxKind.LessThanToken:
                    return JsSyntaxToken.LessThan;
                default:
                    throw new JsTranspilationException<SyntaxToken, JsSyntaxToken>(token);
            }
        }

        public JsSyntax ToJs(SyntaxNode node)
        {
            switch (node)
            {
                case LocalDeclarationStatementSyntax declaration:
                    return ToJsVariableDeclaration(declaration);
                case BlockSyntax block:
                    return ToJsBlock(block);
                default:
                    throw new JsTranspilationException<SyntaxNode, JsSyntax>(node);
            }
        }
    }
}
