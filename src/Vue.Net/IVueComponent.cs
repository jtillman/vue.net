using Newtonsoft.Json;

namespace Vue.Net
{
    [JsonConverter(typeof(VueComponentConverter))]
    public interface IVueComponent
    {
        string Name { get; }

        IVueOptions Options { get; }
    }
}
