using Newtonsoft.Json;

namespace Vue.Net
{
    [JsonConverter(typeof(VueDataConverter))]
    public class VueData
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
