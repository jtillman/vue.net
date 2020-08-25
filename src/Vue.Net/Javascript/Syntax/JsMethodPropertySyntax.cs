using System.IO;
using System.Linq;

namespace Vue.Net.Javascript.Syntax
{
    public class JsMethodPropertySyntax : JsBaseObjectPropertySyntax
    {
        public JsSyntaxToken[] Arguments { get; }

        public JsBlockSyntax Body { get; }

        public JsSyntaxToken[] Modifiers { get; }

        public JsMethodPropertySyntax(
            JsSyntaxToken identifier,
            JsSyntaxToken[] arguments,
            JsBlockSyntax body,
            JsSyntaxToken[] modifiers) : base(identifier)
        {
            Arguments = arguments;
            Body = body;
            Modifiers = modifiers;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            foreach(var modifier in Modifiers)
            {
                textWriter.WriteJsSyntax(modifier, JsSyntaxToken.Space);
            }

            textWriter.WriteJsSyntax(Identifier, JsSyntaxToken.OpenParen);

            if (Arguments.Length > 0)
            {
                textWriter.WriteJsSyntax(Arguments[0]);
                foreach (var argument in Arguments.Skip(1).ToArray())
                {
                    textWriter.WriteJsSyntax(JsSyntaxToken.Comma, JsSyntaxToken.Space, argument);
                }
            }

            textWriter.WriteJsSyntax(JsSyntaxToken.CloseParen);

            if (null == Body)
            {
                textWriter.WriteJsSyntax(JsSyntaxToken.OpenBrace, JsSyntaxToken.CloseBrace, JsSyntaxToken.NewLine);
            }
            else
            {
                textWriter.WriteJsSyntax(Body);
            }
        }
    }
}
