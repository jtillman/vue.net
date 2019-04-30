using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vue.Net.Javascript;

namespace Vue.Net
{
    [HtmlTargetElement("vue-application")]
    public class VueApplicationTagHelper : TagHelper
    {
        public const string IdAttributeName = "id";

        private readonly IVueEngine _vueEngine;

        private readonly VueGlobalOptions _options;

        private readonly IHttpRequestJavascriptBuffer _javascriptBuffer;

        private readonly IServiceProvider _serviceProvider;

        private readonly IRazorViewEngine _viewEngine;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public VueApplicationTagHelper(
            IServiceProvider serviceProvider,
            IHttpRequestJavascriptBuffer javascriptBuffer,
            IOptions<VueGlobalOptions> options,
            IRazorViewEngine viewEngine,
            IVueEngine vueEngine)
        {
            _javascriptBuffer = javascriptBuffer;
            _serviceProvider = serviceProvider;
            _vueEngine = vueEngine;
            _viewEngine = viewEngine;
            _options = options.Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var id = $"vue{Guid.NewGuid().ToString("N")}";
            output.Attributes.SetAttribute(IdAttributeName, id);

            var usedComponents = new List<RegisteredComponent>();
            var componentUsedHandler = new EventHandler<RegisteredComponent>((obj, component) =>
            {
                if (usedComponents.Contains(component))
                {
                    return;
                }
                usedComponents.Add(component);
            });

            _vueEngine.ComponentUsed += componentUsedHandler;
            
            var childContent = await output.GetChildContentAsync();
            output.Content.AppendHtml(childContent);
            _vueEngine.ComponentUsed -= componentUsedHandler;

            var componentDefinitions = usedComponents
                .Select(used => {
                    var instance = ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, used.ModelType);
                    used.Options.Name = used.Options.Name ?? used.ModelType.Name;

                    var templateGenerator = _viewEngine.FindView(ViewContext, $"Components\\{used.ModelType.Name}", false);
                    var viewData = new ViewDataDictionary<object>(ViewContext.ViewData, instance);

                    using (var txtWriter = new StringWriter())
                    {
                        templateGenerator.View.RenderAsync(
                            new ViewContext(
                                ViewContext,
                                templateGenerator.View,
                                viewData,
                                txtWriter))
                                .GetAwaiter()
                                .GetResult();
                        used.Options.Template = txtWriter.ToString();
                    }

                    return new CompiledVueModelComponent(
                        used.ModelType,
                        instance,
                        used.Options,
                        _options.TypeHandler);
                        }).ToArray();

            var content = new VueApplicationContent(
                _options.Serializer,
                id,
                new VueOptions
                {
                    El = $"#{id}"
                },
                componentDefinitions);

            _javascriptBuffer.Add(content);
        }
    }
}
