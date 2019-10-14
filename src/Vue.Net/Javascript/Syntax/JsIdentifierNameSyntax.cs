using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsIdentifierNameSyntax : JsExpressionSyntax
    {
        public JsSyntaxToken Token { get; }

        public JsIdentifierNameSyntax(JsSyntaxToken token)
        {
            Token = token ?? throw new System.ArgumentNullException(nameof(token));
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Token.WriteTo(textWriter);
        }
    }
}
