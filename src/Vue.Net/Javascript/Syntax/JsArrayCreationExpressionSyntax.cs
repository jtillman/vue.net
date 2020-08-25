using System.IO;

namespace Vue.Net.Javascript.Syntax
{

    public sealed class JsArrayCreationExpressionSyntax : JsExpressionSyntax
    {

        public JsArgumentListSyntax Values { get; }

        public JsArrayCreationExpressionSyntax(JsExpressionSyntax[] values) : this(new JsArgumentListSyntax(values)) { }

        public JsArrayCreationExpressionSyntax(JsArgumentListSyntax values) => Values = values;

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(JsSyntaxToken.OpenBracket, Values, JsSyntaxToken.CloseBracket);
        }
    }
}
