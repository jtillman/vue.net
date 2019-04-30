using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueModelConverter : JavascriptConverter<VueModel>
    {
        public override void WriteJavascript(JavascriptTextWriter writer, VueModel value, JsonSerializer serializer)
        {

            var modelType = value.GetType();
            var modelName = modelType.Name;
            var options = new VueOptions();

            var component = new VueComponent( modelType.Name, options);

            serializer.Serialize(writer, component);
        }
    }
}
