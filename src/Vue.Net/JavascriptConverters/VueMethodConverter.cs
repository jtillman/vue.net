using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueMethodConverter : JavascriptConverter<VueMethod>
    {
        public override void WriteJavascript(OldJavascriptTextWriter writer, VueMethod value, JsonSerializer serializer)
        {
            var syntaxWalker = new JavascriptSyntaxWriter(writer);
            syntaxWalker.Visit(value.Declaration);
        }
    }
}
