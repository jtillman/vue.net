using System;
using System.IO;
using System.Linq;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsStatementListSyntax : JsStatementSyntax
    {
        public JsStatementSyntax[] Statements { get; }
        public JsStatementListSyntax(params JsStatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(Statements);
        }
    }

    public sealed class JsSeperatedSyntaxList<T> : JsSyntax where T : JsSyntax
    {
        public JsSyntaxToken Seperator { get; }

        public T[] Items { get; }

        public JsSeperatedSyntaxList(JsSyntaxToken seperator, params T[] items)
        {
            Items = items;
            Seperator = seperator;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            if (Items.Length == 0) return;

            textWriter.WriteJsSyntax(Items[0]);
            foreach(var item in Items.Skip(1).ToArray())
            {
                textWriter.WriteJsSyntax(Seperator, item);
            }
        }
    }

    public sealed class JsBlockSyntax : JsStatementSyntax
    {
        public static JsBlockSyntax Empty => new JsBlockSyntax(new JsSeperatedSyntaxList<JsStatementSyntax>(JsSyntaxToken.NewLine));

        public JsSeperatedSyntaxList<JsStatementSyntax> Statements { get; }

        public JsBlockSyntax(JsStatementSyntax[] statements) : this(new JsSeperatedSyntaxList<JsStatementSyntax>(null, statements)) { }

        public JsBlockSyntax(JsSeperatedSyntaxList<JsStatementSyntax> statements)
        {
            Statements = statements;
        }

        public override void WriteTo(TextWriter textWriter)
        {
            textWriter.WriteJsSyntax(JsSyntaxToken.OpenBrace, JsSyntaxToken.NewLine, Statements, JsSyntaxToken.CloseBrace, JsSyntaxToken.NewLine);
        }
    }
}
