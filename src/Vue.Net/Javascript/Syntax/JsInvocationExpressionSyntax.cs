using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace Vue.Net.Javascript.Syntax
{

    public sealed class JsInvocationExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsArgumentListSyntax Arguments { get; }

        public JsInvocationExpressionSyntax(
            JsExpressionSyntax expression,
            JsArgumentListSyntax arguments)
        {
            Expression = expression;
            Arguments = arguments;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Expression, JsSyntaxToken.OpenParen, Arguments, JsSyntaxToken.CloseParen);
        }
    }
}
