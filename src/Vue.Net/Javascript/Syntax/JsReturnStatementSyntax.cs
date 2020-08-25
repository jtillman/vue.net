using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsReturnStatementSyntax : JsStatementSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsReturnStatementSyntax(JsExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(JsSyntaxToken.Return);
            if (null != Expression)
            {
                textWriter.WriteJsSyntax(JsSyntaxToken.Space, Expression);
            }
            textWriter.WriteJsSyntax(JsSyntaxToken.Semicolon, JsSyntaxToken.NewLine);
        }
    }
}
