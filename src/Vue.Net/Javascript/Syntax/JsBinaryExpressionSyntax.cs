using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public class JsWeakMap
    {
        public JsSyntaxToken Identifier { get; }
            = JsSyntaxToken.Identifier("WeakMap");
    }

    public class JsStringType
    {
        public int length { get; }
    }
    public sealed class JsBinaryExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Left { get; }

        public JsExpressionSyntax Right { get; }

        public JsSyntaxToken OperatorToken { get; }

        public JsBinaryExpressionSyntax(
            JsSyntaxToken operatorToken,
            JsExpressionSyntax left,
            JsExpressionSyntax right) {

            OperatorToken = operatorToken;
            Left = left;
            Right = right;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Left, JsSyntaxToken.Space, OperatorToken, JsSyntaxToken.Space, Right);
        }
    }
}
