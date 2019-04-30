using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vue.Net.Javascript
{
    public class JavascriptWriterState : IDisposable
    {
        internal JavascriptTextWriter Writer { get; set; }

        internal JavascriptWriterState ParentState { get; set; }

        private int? _level;
        public int Level
        {
            get
            {
                if (!_level.HasValue)
                {
                    _level = (ParentState?.Level ?? 0) + (PaddingEnabled && !IsClosed ? 1 : 0);
                }
                return _level.Value;
            }
        }
        
        public string DelimiterText { get; }

        public string PaddingText => PaddingEnabled ? null : string.Empty.PadRight(Level * 2, '-');

        public bool PaddingEnabled => DelimiterText == "\r\n";

        public string ClosingText { get; }

        public string EntryText { get; }

        public bool WaitingOne { get; private set; }

        public bool IsClosed { get; private set; }

        public JavascriptWriterState(string closingText, string delimiterText, string entryText)
        {
            EntryText = entryText;
            DelimiterText = delimiterText;
            ClosingText = closingText;
            WaitingOne = ParentState?.IsClosed ?? true;
        }

        public void Write(string text)
        {
            Writer.WriteJs(text);
        }

        internal void WriteStartNewEntry(bool writeDelimiter)
        {
            if (writeDelimiter && null != DelimiterText)
            {
                Writer.WriteLine();
            }
            if (null != EntryText)
            {
                Write(EntryText);
            }
        }
        public void WriteStartEntry()
        {
            WriteStartNewEntry(!WaitingOne);
            WaitingOne = false;
        }

        public void Close()
        {
            if (!IsClosed && null != ClosingText)
            {
                WriteStartNewEntry(false);
                Write(ClosingText);
                Writer.States.Remove(this);
            }
            IsClosed = true;
        }

        public void Dispose()
        {
            Close();
        }
    }
    public class Clause
    {
        public string Id { get; set; }

        public string[] Modifiers { get; set; }

        public string Identifier { get; set; }

        public string Delimiter { get; set; }

        public char? StartingToken { get; set; }

        public bool Vertical { get; set; }

        public int ValueIndex { get; set; }
    }

    public class JavascriptTextWriter : JsonTextWriter
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

        public JavascriptWriterState State => States.Count > 0 ? States[States.Count - 1] : null;

        internal IList<JavascriptWriterState> States
            = new List<JavascriptWriterState>();

        internal Stack<Clause> OpenClauses = new Stack<Clause>();

        public JavascriptTextWriter(
            TextWriter writer) : base(writer)
        {
            _writer = writer;
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

            clause.Id = Guid.NewGuid().ToString();
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
            WriteStartJsValue();

            clause.Id = Guid.NewGuid().ToString();
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

        public override void Close()
        {
            States
                .Reverse()
                .Where(state => !state.IsClosed)
                .ToList()
                .ForEach(state => state.Close());
            base.Close();
        }
    }

    public class OldJavascriptTextWriter : StringWriter
    {
        public const char StartCurlyBraceToken = '{';
        public const char EndCurlyBraceToken = '}';
        public const char StartBracketToken = '[';
        public const char EndBracketToken = ']';
        public const char SemicolonToken = ';';
        public const char ColonToken = ':';
        public const string IfToken = "if";
        public const char CommaToken = ',';
        public const char StartParenthesisToken = '(';
        public const char EndParenthesisToken = ')';
        public const string ElseConditionToken = "else ";
        public const string VariableDeclarationToken = "var";
        public const string SpaceToken = " ";
        public const char EqualSignToken = '=';
        public const string NewToken = "new";
        public const string ReturnToken = "return";
        public const string NullToken = "null";
        public const char PeriodToken = '.';
        public const string ThisToken = "this";
        public const string AsyncToken = "async";
        public const string AwaitToken = "await";

        public static char[] StartingStatementCharacters = new char[] { StartCurlyBraceToken, StartParenthesisToken, EqualSignToken, StartBracketToken, SemicolonToken };

        public OldJavascriptTextWriter(): base() { }

        public Task WriteStartObjectAsync() => WriteAsync(StartCurlyBraceToken);
        public Task WriteEndObjectAsync() => WriteAsync(EndCurlyBraceToken);
        public Task WriteStartInvocationAsync() => WriteAsync(StartParenthesisToken);

        public Task WriteStartAsyncInvocationAsync(string name) => WriteStartInvocationAsync(name, AsyncToken);

        public Task WriteStartAwaitInvocationAsync(string name) => WriteStartInvocationAsync(name, AwaitToken);

        public async Task WriteStartInvocationAsync(string name, params string[] modifiers)
        {
            await WriteCommaIfRequired();

            foreach (var modifier in modifiers)
            {
                await WriteAsync(modifier);
                await WriteSpaceAsync();
            }

            await WriteAsync(name);
            await WriteStartInvocationAsync();
        }

        public Task WriteEndInvocationAsync() => WriteAsync(EndParenthesisToken);
        public Task WriteCommaAsync() => WriteAsync(CommaToken);
        public Task WriteSpaceAsync() => WriteAsync(SpaceToken);
        public Task WriteVarAsync() => WriteAsync(VariableDeclarationToken);
        public Task WriteEqualSignAsync() => WriteAsync(EqualSignToken);
        public Task WriteReturnAsync() => WriteAsync(ReturnToken);
        public Task WriteNullAsync() => WriteAsync(NullToken);
        public Task WriteNewAsync() => WriteAsync(NewToken);
        public Task WriteThisAsync() => WriteAsync(ThisToken);
        public Task WritePeriodAsync() => WriteAsync(PeriodToken);
        public Task WriteSemicolonAsync() => WriteAsync(SemicolonToken);
        public Task WriteColonAsync() => WriteAsync(ColonToken);
        public Task WriteStartBracketAsync() => WriteAsync(StartBracketToken);
        public Task WriteEndBracketAsync() => WriteAsync(EndBracketToken);

        public async Task WriteStartIfConditionAsync() {
            await WriteAsync(IfToken);
            await WriteStartInvocationAsync();
        }
        public Task WriteEndIfConditionAsync() => WriteAsync(EndParenthesisToken);
        public Task WriteElseConditionAsync() => WriteAsync(ElseConditionToken);
        public Task WriteEndMethodSignatureAsync() => WriteEndObjectAsync();

        public async Task WriteStartVariableDeclartionAsync(string name)
        {
            await WriteVarAsync();
            await WriteSpaceAsync();
            await WriteAsync(name);
            await WriteEqualSignAsync();
        }

        public Task WriteEndVariableDeclartionAsync() => WriteSemicolonAsync();

        public Task WriteStartConstructorAsync(string typeName) => WriteStartInvocationAsync(typeName, NewToken);

        public Task WriteEndConstructorAsync() => WriteEndInvocationAsync();

        public async Task WriteStartIndexerAsync(string name)
        {
            await WriteAsync(name);
            await WriteStartBracketAsync();
        }

        public Task WriteEndIndexerAsync() => WriteEndBracketAsync();

        public async Task WriteIndexerAsync(string name, object index)
        {
            await WriteStartIndexerAsync(name);
            await WriteRawValueAsync(index);
            await WriteEndIndexerAsync();
        }

        public Task WriteValueAsync(string value) => WriteAsync(value);

        public Task WriteRawValueAsync(object value) => WriteValueAsync(JsonConvert.SerializeObject(value));

        public async Task WriteParameterNameAsync(string name)
        {
            await WriteCommaIfRequired();
            await WriteAsync(name);
        }

        public async Task WritePropertyNameAsync(string name)
        {
            await WriteParameterNameAsync(name);
            await WriteColonAsync();
            await WriteSpaceAsync();
        }

        public async Task WriteParameterValueAsync(string value)
        {
            await WriteCommaIfRequired();
            await WriteValueAsync(value);
        }

        public async Task WriteParameterRawValueAsync(object value)
        {
            await WriteCommaIfRequired();
            await WriteRawValueAsync(value);
        }

        public async Task WriteCommaIfRequired()
        {
            var sb = GetStringBuilder();
            var pointer = sb.Length - 1;
            while(pointer >= 0){
                var character = sb[pointer];
                if (!char.IsWhiteSpace(character))
                {
                    if (!StartingStatementCharacters.Contains(character))
                    {
                        await WriteCommaAsync();
                    }
                    return;
                }
                pointer--;
            }
        }
    }
}
