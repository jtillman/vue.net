using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsAwaitExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsAwaitExpressionSyntax(JsExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.Await.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Expression.WriteTo(textWriter);
        }
    }
}
