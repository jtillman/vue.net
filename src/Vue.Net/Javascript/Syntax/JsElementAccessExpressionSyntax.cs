using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsElementAccessExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsArgumentListSyntax Arguments { get; }

        public JsElementAccessExpressionSyntax(JsExpressionSyntax expression, JsExpressionSyntax[] arguments)
        {
            Expression = expression;
            Arguments = new JsArgumentListSyntax(arguments);
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Expression, JsSyntaxToken.OpenBracket, Arguments, JsSyntaxToken.CloseBracket);
        }
    }
}
