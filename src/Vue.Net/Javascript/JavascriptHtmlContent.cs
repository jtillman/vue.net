using Microsoft.AspNetCore.Html;
using System.IO;
using System.Text.Encodings.Web;

namespace Vue.Net.Javascript
{
    public class JavascriptHtmlContent : IHtmlContent
    {
        public IJavascriptContent[] ScriptContents { get; }
        public JavascriptHtmlContent(
            IJavascriptContent[] scriptContents)
        {
            ScriptContents = scriptContents;
        }
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            var jsWriter = new JavascriptTextWriter(writer);
            foreach(var jsContent in ScriptContents)
            {
                jsContent.WriterTo(jsWriter);
            }
        }
    }
}
