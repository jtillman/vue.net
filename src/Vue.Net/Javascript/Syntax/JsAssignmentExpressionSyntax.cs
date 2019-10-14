using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public sealed class JsAssignmentExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Left { get; }

        public JsExpressionSyntax Right { get; }

        public JsAssignmentExpressionSyntax(
            JsExpressionSyntax left,
            JsExpressionSyntax right)
        {
            Left = left;
            Right = right;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Left.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            JsSyntaxToken.EqualSign.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Right.WriteTo(textWriter);
        }
    }
}
