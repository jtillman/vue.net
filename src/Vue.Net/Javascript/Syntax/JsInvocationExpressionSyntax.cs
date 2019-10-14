using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsInvocationExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsExpressionSyntax[] Arguments { get; }
        public JsInvocationExpressionSyntax(
            JsExpressionSyntax expression,
            JsExpressionSyntax[] arguments)
        {
            Expression = expression;
            Arguments = arguments;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Expression.WriteTo(textWriter);
            JsSyntaxToken.OpenParen.WriteTo(textWriter);

            for(int i = 0; i < Arguments.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                Arguments[i].WriteTo(textWriter);
            }

            JsSyntaxToken.CloseParen.WriteTo(textWriter);
        }
    }
}
