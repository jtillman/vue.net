using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsBlockSyntax : JsStatementSyntax
    {
        public JsStatementSyntax[] Statements { get; }

        public JsBlockSyntax(JsStatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.OpenBrace.WriteTo(textWriter);
            foreach (var statement in Statements)
            {
                statement.WriteTo(textWriter);
            }
            JsSyntaxToken.CloseBrace.WriteTo(textWriter);
        }
    }
}
