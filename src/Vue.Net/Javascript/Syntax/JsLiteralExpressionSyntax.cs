using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public class JsLiteralExpressionSyntax : JsExpressionSyntax
    {
        public JsSyntaxToken Token { get; }

        public JsLiteralExpressionSyntax(JsSyntaxToken token)
        {
            Token = token;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Token.WriteTo(textWriter);
        }
    }
}
