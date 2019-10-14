using System;
using System.IO;
using System.Linq;

namespace Vue.Net.Javascript.Syntax
{
    public abstract class JsBaseObjectPropertySyntax : JsSyntax
    {
        public JsSyntaxToken Identifier { get; }

        public JsBaseObjectPropertySyntax(JsSyntaxToken identifier)
        {
            Identifier = identifier;
        }
    }

    public class JsObjectPropertySyntax : JsBaseObjectPropertySyntax
    {

        public JsExpressionSyntax Value { get; }

        public JsObjectPropertySyntax(JsSyntaxToken identifier, JsExpressionSyntax value) : base(identifier)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Identifier.WriteTo(textWriter);
            JsSyntaxToken.Colon.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Value.WriteTo(textWriter);
        }
    }

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
                modifier.WriteTo(textWriter);
                JsSyntaxToken.Space.WriteTo(textWriter);
            }

            Identifier.WriteTo(textWriter);

            JsSyntaxToken.OpenParen.WriteTo(textWriter);

            if (Modifiers.Length > 0)
            {
                Modifiers[0].WriteTo(textWriter);

                foreach(var modifier in Modifiers.Skip(1))
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                    modifier.WriteTo(textWriter);
                }
            }
            JsSyntaxToken.CloseParen.WriteTo(textWriter);
            Body.WriteTo(textWriter);
        }
    }
}
