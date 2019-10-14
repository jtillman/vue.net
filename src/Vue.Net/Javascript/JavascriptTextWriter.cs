using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vue.Net.Javascript
{
    public static class JsToken
    {
        public static readonly string OpenCurlyBrace = "{";
        public static readonly string CloseCurlyBrace = "}";

        public static readonly string OpenBracket = "[";
        public static readonly string CloseBracket = "]";

        public static readonly string OpenParenthisis = "(";
        public static readonly string CloseParenthisis = ")";
    }

    public enum JavascriptState
    {
        Document,
        Object,
        ObjectProperty,
        MethodSignature,
        MethodBody,
        Statement
    }

    public class JavascriptTextWriter : JsonTextWriter
    {
        private readonly TextWriter _writer;

        public JavascriptFormattingOptions FormattingOptions { get; }

        internal readonly Stack<JavascriptState> _openStates = new Stack<JavascriptState>();

        internal JavascriptState State => _openStates.Peek();

        private bool _isFirstValue = true;

        public JavascriptTextWriter(
            TextWriter textWriter,
            JavascriptFormattingOptions formattingOptions = null) : base(textWriter)
        {
            _writer = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
            FormattingOptions = formattingOptions ?? JavascriptFormattingOptions.Default;
            PushState(JavascriptState.Document);
        }

        public void WriteJsValue(int value)
        {
            AdvanceStateValue();
            _writer.Write(value);
        }

        internal void PushState(JavascriptState state)
        {
            _openStates.Push(state);
            _isFirstValue = true;
        }
        
        internal void PopState()
        {
            _openStates.Pop();
            _isFirstValue = false;
        }

        internal void AdvanceStateValue()
        {
            if (_isFirstValue)
            {
                _isFirstValue = false;
                return;
            }

            switch (State)
            {
                case JavascriptState.Document:
                    _writer.Write(FormattingOptions.NewLineString);
                    break;
                case JavascriptState.Object:
                case JavascriptState.ObjectProperty:
                    _writer.Write(",");
                    _writer.Write(FormattingOptions.NewLineString);
                    break;
                case JavascriptState.MethodSignature:
                    _writer.Write(", ");
                    break;
                case JavascriptState.MethodBody:
                    _writer.Write(FormattingOptions.NewLineString);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void WriteStartJsObject(){
            AdvanceStateValue();
            _writer.Write("{");
            PushState(JavascriptState.Object);
        }

        public void WriteEndJsObject() {
            switch (State)
            {
                case JavascriptState.Object:
                    break;
                case JavascriptState.ObjectProperty:
                    break;
                case JavascriptState.MethodSignature:
                    break;
                default:
                    throw new InvalidOperationException();
            }
            _writer.Write("}");
        }

        public void WriteStartJsObjectProperty(string propertyName) {

            switch (State)
            {
                case JavascriptState.Object:
                    break;
                case JavascriptState.ObjectProperty:
                    if (_isFirstValue)
                    {
                        throw new InvalidOperationException("Expected Property Value");
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            AdvanceStateValue();
            _writer.Write(propertyName);
            _writer.Write(": ");

            PushState(JavascriptState.ObjectProperty);
        }

        public void WriteStartJsMethodDefinition(string methodName)
        {
            AdvanceStateValue();
            _writer.Write(methodName);
            _writer.Write("(");
            PushState(JavascriptState.MethodSignature);
        }

        public void WriteEndJsMethodDefinition() {
            switch (State)
            {
                case JavascriptState.MethodSignature:
                    _writer.Write(") {}");
                    break;
                case JavascriptState.MethodBody:
                    _writer.Write("}");
                    break;
                default:
                    throw new InvalidOperationException();
            }
            PopState();
        }

        public void WriteJsMethodArgument(string argumentName)
        {
            if (State != JavascriptState.MethodSignature)
            {
                throw new InvalidOperationException();
            }

            AdvanceStateValue();
            _writer.Write(argumentName);
        }

        internal void WriteStartStatement()
        {
        }
        public void WriteStartVariableDeclarationStatement(string variableName)
        {
            WriteStartStatement();
            _writer.Write(variableName);
            _writer.Write(" = ");
        }

        public void WriteEndStatement()
        {

        }
    }

    public class Clause
    {
        public string[] Modifiers { get; set; }

        public string Identifier { get; set; }

        public string Delimiter { get; set; }

        public char? StartingToken { get; set; }

        public bool Vertical { get; set; }

        public int ValueIndex { get; set; }
    }

    public class JavascriptFormattingOptions
    {
        public static JavascriptFormattingOptions Default =
            new JavascriptFormattingOptions();

        public string NewLineString { get; }
        public char PaddingCharcter { get; }
        public string CurlyBracePostDelimiter { get; }
        public string BracketPostDelimiter { get; }
        public string ParenthisisPostDelmiter { get; }

        public JavascriptFormattingOptions(
            string newLineString = "\r\n",
            char paddingCharacter = ' ',
            string curlyBracePostDelimiter = "\r\n",
            string bracketPostDelimiter = " ",
            string parenthisisPostDelimiter = " "
            )
        {
            NewLineString = newLineString;
            PaddingCharcter = paddingCharacter;
            CurlyBracePostDelimiter = curlyBracePostDelimiter;
            BracketPostDelimiter = bracketPostDelimiter;
            ParenthisisPostDelmiter = parenthisisPostDelimiter;
        }
    }

    public enum JavascriptEnclosureType
    {
        CurlyBrace,
        Parenthesis,
        Bracket
    }

    public class OldJavascriptTextWriter : JsonTextWriter
    {
        public const string AsyncModifier = "async ";
        public const string Comma = ",";
        public const string OpenBracket = "[";
        public const string CloseBracket = "]";
        public const string OpenCurlyBrace = "{";
        public const string CloseCurlyBrace = "}";
        public const string OpenParenthesis = "(";
        public const string CloseParenthesis = ")";

        private readonly TextWriter _writer;

        private JavascriptFormattingOptions FormattingOptions { get; }

        internal Stack<Clause> OpenClauses = new Stack<Clause>();

        public OldJavascriptTextWriter(
            TextWriter writer,
            JavascriptFormattingOptions formattingOptions = null
            ) : base(writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            FormattingOptions = formattingOptions ?? JavascriptFormattingOptions.Default;
        }

        public void WriteStartJsValue()
        {
            var clause = OpenClauses.Count > 0 ? OpenClauses.Peek() : null;
            if (null == clause) return;

            if (clause.ValueIndex > 0)
            {
                Write(clause.Delimiter);
            }

            if (clause.Vertical)
            {
                WriteLine();
            }

            clause.ValueIndex++;
        }

        public void WriteStartJsClause(char clauseHeader, string identifier = null, string[] modifiers = null, bool vertical = false)
        {
            var clause = new Clause
            {
                StartingToken = clauseHeader,
                Identifier = identifier ?? string.Empty,
                Modifiers = modifiers ?? new string[0],
                Vertical = vertical
            };
            WriteStartJsClause(clause);
        }

        public void WriteStartJsClause(Clause clause)
        {

            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause));
            }

            WriteStartJsValue();

            if (null != clause.Modifiers)
            {
                foreach (var modifier in clause.Modifiers)
                {
                    Write(modifier);
                    Write(" ");
                }
            }

            if (null != clause.Identifier)
            {
                Write(clause.Identifier);
            }
            
            if (clause.StartingToken.HasValue)
            {
                Write(clause.StartingToken.Value);
            }
            
            OpenClauses.Push(clause);
        }

        public void WriteEndJsClause(char? expectedStartingToken = null, char? endingToken = null)
        {
            var lastClause = OpenClauses.Peek();
            if (lastClause.StartingToken != expectedStartingToken)
            {
                throw new NotSupportedException($"Can't close clause because we expected {expectedStartingToken} and found {lastClause.StartingToken}");
            }

            OpenClauses.Pop();

            if (endingToken.HasValue)
            {
                if (lastClause.Vertical)
                {
                    WriteLine();
                }
                Write(endingToken.Value);
            }
        }

        public virtual void Write(char character) => _writer.Write(character);

        public virtual void Write(string text) => _writer.Write(text);

        internal void WriteLine()
        {
            Write("\r\n");
            WritePadding();
        }
        internal void WritePadding()
        {
            Write(string.Empty.PadRight(OpenClauses.Count(c => c.Vertical) * 4, ' '));
        }

        internal void WriteJs(string text)
        {
            Write(text);
        }

        public void WriteJsValue(object value)
        {
            WriteStartJsValue();
            base.WriteValue(value);
        }

        public void WriteJsRawValue(string value)
        {
            WriteStartJsValue();
            WriteJs(value);
        }

        public void WriteStartJsObject() =>
            WriteStartJsClause(
                new Clause
                {
                    StartingToken = '{',
                    Delimiter = ", ",
                    Vertical = true
                });

        public void WriteEndJsObject() => WriteEndJsClause('{', '}');

        public void WriteStartJsFunctionDefinition() =>
            WriteStartJsClause(
                new Clause
                {

                });

        public void WriteEndJsFunctionDefinition() => WriteEndJsClause();

        public void WriteStartJsFunctionBlock() =>
            WriteStartJsClause(
                new Clause
                {
                    StartingToken = '{',
                    Vertical = true
                });

        public void WriteEndJsFunctionBlock() => WriteEndJsClause('{', '}');

        public void WriteStartJsIfCondition() => WriteStartJsClause(
            new Clause
            {
                Identifier = "if",
                StartingToken = '(',
            });

        public void WriteEndJsIfCondition() => WriteEndJsClause('(', ')');

        public void WriteStartJsFunctionSignature(string name, params string[] modifiers) =>
            WriteStartJsClause(
                new Clause
                {
                    Identifier = name,
                    Modifiers = modifiers,
                    StartingToken = '(',
                    Delimiter = ", "
                });

        public void WriteEndJsFunctionSignature() => WriteEndJsClause('(', ')');

        public void WriteStartJsMember(string name)
        {
            WriteStartJsClause(new Clause {
                Delimiter = ": "});
            WriteJsRawValue(name);
        }


        public void WriteEndJsMember() => WriteEndJsClause();

        public void WriteStartJsStatement(params string[] modifiers) =>
            WriteStartJsClause(
                new Clause
                {
                    Modifiers = modifiers,
                });

        public void WriteEndJsStatement() => WriteEndJsClause(null, ';');

        public void WriteStartJsVariableStatement(string variableName)
        {
            WriteStartJsClause(
                new Clause
                {
                    Modifiers = new string[] { "var" },
                    Identifier = variableName,
                    Delimiter = " "

                });

            WriteJsRawValue("=");

        }


        public void WriteStartJsReturnStatement() => WriteStartJsStatement("return");

        public void WriteComma() => _writer.Write(Comma);
        public void WriteCloseBracket() => _writer.Write(CloseBracket);
        public void WriteCloseCurlyBrace() => _writer.Write(CloseCurlyBrace);
        public void WriteCloseParenthesis() => _writer.Write(CloseParenthesis);
        public void WriteOpenBracket() => _writer.Write(OpenBracket);
        public void WriteOpenCurlyBrace() => _writer.Write(OpenCurlyBrace);
        public void WriteOpenParenthesis() => _writer.Write(OpenParenthesis);
    }
}
