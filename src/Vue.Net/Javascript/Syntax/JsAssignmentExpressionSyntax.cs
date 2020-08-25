using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public sealed class JsAssignmentExpressionSyntax : JsExpressionSyntax
    {
        public JsSyntaxToken OperatorToken { get; }

        public JsExpressionSyntax Left { get; }

        public JsExpressionSyntax Right { get; }

        public JsAssignmentExpressionSyntax(
            JsSyntaxToken operatorToken,
            JsExpressionSyntax left,
            JsExpressionSyntax right)
        {
            OperatorToken = operatorToken;
            Left = left;
            Right = right;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(
                Left,
                JsSyntaxToken.Space,
                OperatorToken,
                JsSyntaxToken.Space,
                Right);
        }
    }
}
