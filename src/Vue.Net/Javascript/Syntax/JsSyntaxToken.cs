using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Web;

namespace Vue.Net.Javascript.Syntax
{
    public class JsSyntaxToken : JsSyntax
    {
        public SyntaxKind GetCSharpSyntaxKind()
        {
            return this.Value switch
            {
                "get" => SyntaxKind.GetKeyword,
                "set" => SyntaxKind.SetKeyword,
                "if" => SyntaxKind.IfKeyword,
                "else" => SyntaxKind.ElseKeyword,
                "\r\n" => SyntaxKind.EndOfLineTrivia,
                "null" => SyntaxKind.NullKeyword,
                "(" => SyntaxKind.OpenParenToken,
                ")" => SyntaxKind.CloseParenToken,
                "[" => SyntaxKind.OpenBracketToken,
                "]" => SyntaxKind.CloseBracketToken,
                "{" => SyntaxKind.OpenBraceToken,
                "}" => SyntaxKind.CloseBraceToken,
                "," => SyntaxKind.CommaToken,
                ":" => SyntaxKind.ColonToken,
                ";" => SyntaxKind.SemicolonToken,
                " " => SyntaxKind.WhitespaceTrivia,
                "throw" => SyntaxKind.ThrowKeyword,
                "async" => SyntaxKind.AsyncKeyword,
                "await" => SyntaxKind.AwaitKeyword,
                "new" => SyntaxKind.NewKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                "." => SyntaxKind.DotToken,
                "this" => SyntaxKind.ThisKeyword,
                "var" => SyntaxKind.VarKeyword,
                "let" => SyntaxKind.LetKeyword,
                "=>" => SyntaxKind.EqualsGreaterThanToken,
                "=" => SyntaxKind.EqualsToken,
                "==" => SyntaxKind.EqualsEqualsToken,
                "&&" => SyntaxKind.AmpersandAmpersandToken,
                "!=" => SyntaxKind.ExclamationEqualsToken,
                "||" => SyntaxKind.BarBarToken,
                "<=" => SyntaxKind.LessThanEqualsToken,
                ">=" => SyntaxKind.GreaterThanEqualsToken,
                ">" => SyntaxKind.GreaterThanToken,
                "<" => SyntaxKind.LessThanToken,
                _ => throw new ArgumentException($"Unknown CSharp Syntax for Token '{Value}'")
            };
        }

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

        public static JsSyntaxToken This =>
            new JsSyntaxToken("this", "this");

        public static JsSyntaxToken Class =>
            new JsSyntaxToken("class", "class");

        public static JsSyntaxToken GlobalThis =>
            new JsSyntaxToken("globalThis", "globalThis");

        public static JsSyntaxToken Var =>
            new JsSyntaxToken("var", "var");

        public static JsSyntaxToken ValueParamterName =>
            new JsSyntaxToken("value", "value");

        public static JsSyntaxToken Let =>
            new JsSyntaxToken("let", "let");

        public static JsSyntaxToken ArrowOperator =>
            new JsSyntaxToken("=>", "=>");

        public static JsSyntaxToken EqualSign =>
            new JsSyntaxToken("=", "=");

        public static JsSyntaxToken EqualsEquals =>
            new JsSyntaxToken("==", "==");

        public static JsSyntaxToken AmpersandAmpersand =>
            new JsSyntaxToken("&&", "&&");

        public static JsSyntaxToken ExclamationEquals =>
            new JsSyntaxToken("!=", "!=");

        public static JsSyntaxToken BarBar =>
            new JsSyntaxToken("||", "||");

        public static JsSyntaxToken LessThanEqual =>
            new JsSyntaxToken("<=", "<=");

        public static JsSyntaxToken GreaterThanEqual =>
            new JsSyntaxToken(">=", ">=");

        public static JsSyntaxToken GreaterThan =>
            new JsSyntaxToken(">", ">");

        public static JsSyntaxToken LessThan =>
            new JsSyntaxToken("<", "<");

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
