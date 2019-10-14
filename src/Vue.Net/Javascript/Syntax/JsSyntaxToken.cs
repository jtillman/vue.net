using System.IO;
using System.Web;

namespace Vue.Net.Javascript.Syntax
{
    public class JsSyntaxToken : JsSyntax
    {
        public object Value { get; }
        public string ValueText { get; }

        internal JsSyntaxToken(object value, string valueText)
        {
            Value = value;
            ValueText = valueText;
        }

        public static JsSyntaxToken Get =>
            new JsSyntaxToken("get", "get");

        public static JsSyntaxToken Set =>
            new JsSyntaxToken("set", "set");

        public static JsSyntaxToken If =>
            new JsSyntaxToken("if", "if");

        public static JsSyntaxToken Else =>
            new JsSyntaxToken("else", "else");

        public static JsSyntaxToken Function =>
            new JsSyntaxToken("function", "function");

        public static JsSyntaxToken NewLine =>
            new JsSyntaxToken("\r\n", "\r\n");

        public static JsSyntaxToken Null =>
            new JsSyntaxToken(null, "null");

        public static JsSyntaxToken OpenParen =>
            new JsSyntaxToken("(", "(");

        public static JsSyntaxToken CloseParen =>
            new JsSyntaxToken(")", ")");

        public static JsSyntaxToken OpenBracket =>
            new JsSyntaxToken("[", "[");

        public static JsSyntaxToken CloseBracket =>
            new JsSyntaxToken("]", "]");

        public static JsSyntaxToken OpenBrace =>
            new JsSyntaxToken("{", "{");

        public static JsSyntaxToken CloseBrace =>
            new JsSyntaxToken("}", "}");

        public static JsSyntaxToken Comma =>
            new JsSyntaxToken(",", ",");

        public static JsSyntaxToken Colon =>
            new JsSyntaxToken(":", ":");

        public static JsSyntaxToken Semicolon =>
            new JsSyntaxToken(";", ";");

        public static JsSyntaxToken Space =>
            new JsSyntaxToken(" ", " ");

        public static JsSyntaxToken Throw =>
            new JsSyntaxToken("throw", "throw");

        public static JsSyntaxToken Async =>
            new JsSyntaxToken("async", "async");

        public static JsSyntaxToken Await =>
            new JsSyntaxToken("await", "await");

        public static JsSyntaxToken New =>
            new JsSyntaxToken("new", "new");

        public static JsSyntaxToken Return =>
            new JsSyntaxToken("return", "return");

        public static JsSyntaxToken Dot =>
            new JsSyntaxToken(".", ".");

        public static JsSyntaxToken Var =>
            new JsSyntaxToken("var", "var");

        public static JsSyntaxToken Let =>
            new JsSyntaxToken("let", "let");

        public static JsSyntaxToken ArrowOperator =>
            new JsSyntaxToken("=>", "=>");

        public static JsSyntaxToken EqualSign =>
            new JsSyntaxToken("=", "=");

        public static JsSyntaxToken NumberLiteral(int value) => new JsSyntaxToken(value, value.ToString());

        public static JsSyntaxToken NumberLiteral(string value) => new JsSyntaxToken(value, value);

        public static JsSyntaxToken StringLiteral(string value) => new JsSyntaxToken(value, HttpUtility.JavaScriptStringEncode(value, true));

        public static JsSyntaxToken BooleanLiteral(bool value) => new JsSyntaxToken(value, value ? "true" : "false");

        public static JsSyntaxToken Identifier(string value) => new JsSyntaxToken(value, value);

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.Write(ValueText);
        }
    }
}
