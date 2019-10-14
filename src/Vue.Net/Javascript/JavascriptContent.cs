using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Vue.Net.Javascript
{
    public class JavascriptContent<TType> : IJavascriptContent
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly VueGlobalOptions _options;

        private TType _instance;
        public TType Instance 
        {
            get
            {
                if (null == _instance)
                {
                    _instance = (TType)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, typeof(TType));
                }
                return _instance;
            }
        }


        public JavascriptContent(
            IServiceProvider serviceProvider,
            IOptions<VueGlobalOptions> options,
            TType instance = default(TType))
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _instance = instance;

        }

        public void WriterTo(OldJavascriptTextWriter writer)
        {
            _options.Serializer.Serialize(writer, Instance);
        }
    }
}
