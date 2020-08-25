using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public class JsThrowStatementSyntax : JsStatementSyntax
    {
        public JsExpressionSyntax Expression { get; }
        public JsThrowStatementSyntax(
            JsExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.Throw.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Expression.WriteTo(textWriter);
            JsSyntaxToken.Semicolon.WriteTo(textWriter);
        }
    }
}
