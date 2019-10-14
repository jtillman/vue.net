using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Vue.Net.Javascript
{
    public class CSharpSyntaxConverter : JavascriptConverter<CompilationUnitSyntax>
    {
        public override void WriteJavascript(OldJavascriptTextWriter writer, CompilationUnitSyntax value, JsonSerializer serializer)
        {
            var syntaxWriter = new JavascriptSyntaxWriter(writer);
            {
                syntaxWriter.Visit(value);
            }
        }
    }
}
