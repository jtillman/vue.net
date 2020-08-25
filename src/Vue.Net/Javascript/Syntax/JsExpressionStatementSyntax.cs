using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsExpressionStatementSyntax : JsStatementSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsExpressionStatementSyntax(JsExpressionSyntax expression) : base()
        {
            Expression = expression;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Expression, JsSyntaxToken.Semicolon, JsSyntaxToken.NewLine);
        }
    }
}
