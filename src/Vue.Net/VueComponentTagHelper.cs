using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Vue.Net
{
    [HtmlTargetElement("*")]
    public class VueComponentTagHelper : TagHelper
    {
        private readonly IVueEngine _vueEngine;

        public VueComponentTagHelper(
            IVueEngine vueEngine)
        {
            _vueEngine = vueEngine;
            
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var elementName = context.TagName;

            var component = _vueEngine.GetComponent(elementName);
            if (null != component)
            {
                _vueEngine.NotifyUsed(component);
            }
            await base.ProcessAsync(context, output);
        }
    }
}
