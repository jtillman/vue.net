using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsObjectCreationExpressionSyntax : JsExpressionSyntax
    {
        public JsBaseObjectPropertySyntax[] Properties { get; }

        public JsObjectCreationExpressionSyntax(
            JsBaseObjectPropertySyntax[] properties)
        {
            Properties = properties;
        }


        public override void WriteTo(TextWriter textWriter)
        {
            JsSyntaxToken.OpenBrace.WriteTo(textWriter);

            for(int i = 0; i < Properties.Length; i++)
            {
                if (i > 0)
                {
                    JsSyntaxToken.Comma.WriteTo(textWriter);
                    JsSyntaxToken.Space.WriteTo(textWriter);
                }
                Properties[i].WriteTo(textWriter);
            }
            JsSyntaxToken.CloseBrace.WriteTo(textWriter);
        }
    }
}
