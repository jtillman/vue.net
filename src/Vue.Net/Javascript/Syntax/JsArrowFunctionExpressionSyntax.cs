using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsArrowFunctionExpressionSyntax : JsExpressionSyntax
    {
        public JsIdentifierNameSyntax[] Parameters { get; }

        public JsSyntax Body { get; }

        public JsArrowFunctionExpressionSyntax(
            JsIdentifierNameSyntax[] parameters,
            JsSyntax body)
        {
            Parameters = parameters;
            Body = body;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.OpenParen.WriteTo(textWriter);
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                Parameters[i].WriteTo(textWriter);
            }
            JsSyntaxToken.CloseParen.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            JsSyntaxToken.ArrowOperator.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Body.WriteTo(textWriter);
        }
    }
}
