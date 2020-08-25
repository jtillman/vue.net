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
            textWriter.WriteJsSyntax(JsSyntaxToken.Var, JsSyntaxToken.Space, Name);

            if (null != Initializer)
            {
                textWriter.WriteJsSyntax(JsSyntaxToken.Space, Initializer);
            }
            textWriter.WriteJsSyntax(JsSyntaxToken.Semicolon, JsSyntaxToken.NewLine);
        }
    }
}
