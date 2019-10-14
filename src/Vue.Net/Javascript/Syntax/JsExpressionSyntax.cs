namespace Vue.Net.Javascript.Syntax
{
    public abstract class JsExpressionSyntax : JsSyntax
    {
        public static JsExpressionSyntax Null { get; }
            = new JsLiteralExpressionSyntax(JsSyntaxToken.Null);
    }
}
