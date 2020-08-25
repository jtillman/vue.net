using System;
using System.Collections.Generic;
using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsClassExpressionSyntax : JsExpressionSyntax
    {
        private IEnumerable<JsSyntax> enumerable;

        public JsSyntax[] Initializers { get; }

        public JsClassExpressionSyntax(JsSyntax[] initializers)
        {
            Initializers = initializers ?? throw new ArgumentNullException(nameof(initializers));
        }

        public JsClassExpressionSyntax(IEnumerable<JsSyntax> enumerable)
        {
            this.enumerable = enumerable;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter
                .WriteJsSyntax(
                    JsSyntaxToken.Class,
                    JsSyntaxToken.Space,
                    JsSyntaxToken.OpenBrace,
                    JsSyntaxToken.NewLine)
                .WriteJsSyntax(Initializers)
                .WriteJsSyntax(JsSyntaxToken.CloseBrace);
        }
    }
}
