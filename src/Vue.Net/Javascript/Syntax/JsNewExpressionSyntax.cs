using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsNewExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsNewExpressionSyntax(JsExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(JsSyntaxToken.New, JsSyntaxToken.Space, Expression);
        }
    }
}
