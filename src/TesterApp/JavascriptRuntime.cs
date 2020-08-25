using Microsoft.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.IO;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{
    public class JavascriptRuntime
    {

        public JsSyntax Syntax { get; }

        public JavascriptRuntime() : this(JsEmptySyntax.Instance) { }
        public JavascriptRuntime(JsSyntax syntax) => Syntax = syntax;

        public override string ToString()
        {
            using (var textWriter = new StringWriter())
            {
                Syntax.WriteTo(textWriter);
                return textWriter.ToString();
            }
        }
    }
}
