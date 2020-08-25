using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsArgumentListSyntax : JsSyntax
    {
        public JsSeperatedSyntaxList<JsExpressionSyntax> Arguments { get; }
        public JsArgumentListSyntax(JsExpressionSyntax[] arguments)
        {
            Arguments = new JsSeperatedSyntaxList<JsExpressionSyntax>(JsSyntaxToken.Comma, arguments);
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Arguments);
        }
    }
}
