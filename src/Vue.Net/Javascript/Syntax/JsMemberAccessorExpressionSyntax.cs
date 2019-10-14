using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public class JsMemberAccessorExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax Expression { get; }

        public JsSyntaxToken Name { get; }

        public JsMemberAccessorExpressionSyntax(
            JsExpressionSyntax expression,
            JsSyntaxToken name)
        {
            Expression = expression;
            Name = name;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            Expression.WriteTo(textWriter);
            JsSyntaxToken.Dot.WriteTo(textWriter);
            Name.WriteTo(textWriter);
        }
    }
}
