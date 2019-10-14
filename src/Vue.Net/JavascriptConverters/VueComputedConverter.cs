using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueComputedConverter : JavascriptConverter<VueComputed>
    {
        public override void WriteJavascript(OldJavascriptTextWriter writer, VueComputed value, JsonSerializer serializer)
        {
            writer.WriteStartJsMember(value.Name);
            writer.WriteStartJsObject();

            var declaration = value.Body.Members.First() as PropertyDeclarationSyntax;
            var syntaxWriter = new JavascriptSyntaxWriter(writer);
            foreach (var accessor in declaration.AccessorList.Accessors)
            {
                syntaxWriter.Visit(accessor);
            }
            writer.WriteEndJsObject();
            writer.WriteEndJsMember();
        }
    }
}
