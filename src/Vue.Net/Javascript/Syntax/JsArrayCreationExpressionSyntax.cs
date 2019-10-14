using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsArrayCreationExpressionSyntax : JsExpressionSyntax
    {
        public JsExpressionSyntax[] Values { get; }
        public JsArrayCreationExpressionSyntax(JsExpressionSyntax[] values)
        {
            Values = values;
        }


        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.OpenBracket.WriteTo(textWriter);

            for(int i = 0; i < Values.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                Values[i].WriteTo(textWriter);
            }
            JsSyntaxToken.CloseBracket.WriteTo(textWriter);
        }
    }
}
