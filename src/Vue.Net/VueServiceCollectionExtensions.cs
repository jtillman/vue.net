using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public static class VueServiceCollectionExtensions
    {
        public static OptionsBuilder<VueGlobalOptions> AddVue(this IServiceCollection services)
        {
            services.AddSingleton<IVueEngine, VueEngine>();
            services.AddScoped<IHttpRequestJavascriptBuffer, HttpRequestJavascriptBuffer>();
            return services.AddOptions<VueGlobalOptions>();
        }
    }
}
