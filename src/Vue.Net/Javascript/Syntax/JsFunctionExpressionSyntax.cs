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
            JsSyntaxToken.Function.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            JsSyntaxToken.OpenParen.WriteTo(textWriter);

            for (int i = 0; i < ArgumentNames.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                ArgumentNames[i].WriteTo(textWriter);
            }
            JsSyntaxToken.CloseParen.WriteTo(textWriter);
            JsSyntaxToken.NewLine.WriteTo(textWriter);
            Body.WriteTo(textWriter);
        }
    }
}
