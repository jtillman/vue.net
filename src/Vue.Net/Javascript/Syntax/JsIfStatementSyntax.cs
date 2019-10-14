using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsIfStatementSyntax : JsStatementSyntax
    {
        public JsExpressionSyntax Condition { get; }

        public JsStatementSyntax Statement { get; }

        public JsElseClauseSyntax Else { get; }

        public JsIfStatementSyntax(
            JsExpressionSyntax condition,
            JsStatementSyntax statement,
            JsElseClauseSyntax elseClause) : base()
        {
            Condition = condition;
            Statement = statement;
            Else = elseClause;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.If.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            JsSyntaxToken.OpenParen.WriteTo(textWriter);
            Condition.WriteTo(textWriter);
            JsSyntaxToken.CloseParen.WriteTo(textWriter);
            JsSyntaxToken.NewLine.WriteTo(textWriter);

            Statement.WriteTo(textWriter);

            if (null != Else)
            {
                JsSyntaxToken.NewLine.WriteTo(textWriter);
                Else.WriteTo(textWriter);
            }
        }
    }
}
