using System;
using System.IO;

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
}
