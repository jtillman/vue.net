using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsParenthesizedExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsParenthesizedExpressionSyntax(JsExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(JsSyntaxToken.OpenParen, Expression, JsSyntaxToken.CloseParen);
        }
    }
}
