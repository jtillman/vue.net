using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public class JsElseClauseSyntax : JsSyntax 
    {
        public JsStatementSyntax Statement { get; }

        public JsElseClauseSyntax(JsStatementSyntax statement)
        {
            Statement = statement;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.Write("else ");
            Statement.WriteTo(textWriter);
        }
    }
    public sealed class JsObjectConstructionExpressionSyntax : JsExpressionSyntax
    {

        public JsSyntaxToken Type { get; }

        public JsExpressionSyntax[] Arguments { get; }

        public JsObjectConstructionExpressionSyntax(
            JsSyntaxToken type,
            JsExpressionSyntax[] arguments)
        {
            Type = type;
            Arguments = arguments;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.New.WriteTo(textWriter);
            JsSyntaxToken.Space.WriteTo(textWriter);
            Type.WriteTo(textWriter);
            JsSyntaxToken.OpenParen.WriteTo(textWriter);

            for(int i = 0; i < Arguments.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                Arguments[i].WriteTo(textWriter);
            }
            JsSyntaxToken.CloseParen.WriteTo(textWriter);
        }
    }
}
