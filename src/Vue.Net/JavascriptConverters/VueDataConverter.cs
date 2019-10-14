using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueDataConverter : JavascriptConverter<VueData>
    {
        public override void WriteJavascript(OldJavascriptTextWriter writer, VueData value, JsonSerializer serializer)
        {
            writer.WriteStartJsMember(value.Name);
            writer.WriteStartJsValue();
            serializer.Serialize(writer, value.Value);
            writer.WriteEndJsMember();
        }
    }
}
