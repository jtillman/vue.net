using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public sealed class JsFunctionExpressionSyntax : JsExpressionSyntax
    {
        public JsIdentifierNameSyntax[] ArgumentNames { get; }

        public JsBlockSyntax Body { get; }

        public JsFunctionExpressionSyntax(
            JsIdentifierNameSyntax[] argumentNames,
            JsBlockSyntax body)
        {
            ArgumentNames = argumentNames;
            Body = body;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(
                JsSyntaxToken.Function,
                JsSyntaxToken.Space,
                JsSyntaxToken.OpenParen);

            for (int i = 0; i < ArgumentNames?.Length; i++)
            {
                if (i > 0)
                {
                    textWriter.WriteJsSyntax(JsSyntaxToken.Comma, JsSyntaxToken.Space);
                }
                textWriter.WriteJsSyntax(ArgumentNames[i]);
            }

            textWriter.WriteJsSyntax(JsSyntaxToken.CloseParen, JsSyntaxToken.NewLine);
            _ = null != Body ? textWriter.WriteJsSyntax(Body) : textWriter.WriteJsSyntax(JsSyntaxToken.OpenBrace, JsSyntaxToken.CloseBrace);
        }
    }
}
