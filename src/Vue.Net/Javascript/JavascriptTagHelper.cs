using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Threading.Tasks;

namespace Vue.Net.Javascript
{
    [HtmlTargetElement("javascript")]
    public class JavascriptTagHelper : TagHelper
    {
        private readonly IHttpRequestJavascriptBuffer _jsBuffer;

        public JavascriptTagHelper(
            IHttpRequestJavascriptBuffer jsBuffer)
        {
            _jsBuffer = jsBuffer;
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Clear();
            output.TagName = "script";
            output.Attributes.SetAttribute("type", "text/javascript");

            var jsContents = _jsBuffer.ToArray();
            _jsBuffer.Clear();

            if (jsContents.Length > 0)
            {
                output.Content.AppendHtml(new JavascriptHtmlContent(jsContents));
            }

            return base.ProcessAsync(context, output);
        }
    }
}
