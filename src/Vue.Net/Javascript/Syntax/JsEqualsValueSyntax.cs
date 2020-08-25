using System;
using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsEqualsValueSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Value { get; }

        public JsEqualsValueSyntax(JsExpressionSyntax value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(
                JsSyntaxToken.EqualSign,
                JsSyntaxToken.Space,
                Value);
        }
    }
}
