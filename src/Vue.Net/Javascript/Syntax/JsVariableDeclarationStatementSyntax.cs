using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsVariableDeclarationStatementSyntax : JsStatementSyntax
    {
        public JsIdentifierNameSyntax Name { get; }

        public JsExpressionSyntax Initializer { get; }

        public JsVariableDeclarationStatementSyntax(
            JsIdentifierNameSyntax name,
            JsExpressionSyntax initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.Var.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Name.WriteTo(textWriter);

            if (null != Initializer)
            {
                JsSyntaxToken.Space.WriteTo(textWriter);
                JsSyntaxToken.EqualSign.WriteTo(textWriter);
                JsSyntaxToken.Space.WriteTo(textWriter);

                Initializer.WriteTo(textWriter);
            }
            JsSyntaxToken.Semicolon.WriteTo(textWriter);
        }
    }
}
