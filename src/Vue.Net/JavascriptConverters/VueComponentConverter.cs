using System;
using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueComponentConverter : JavascriptConverter<IVueComponent>
    {
        public override void WriteJavascript(
            JavascriptTextWriter writer,
            IVueComponent value,
            JsonSerializer serializer)
        {
            writer.WriteStartJsFunctionDefinition();
            writer.WriteStartJsFunctionSignature("Vue.component");
            writer.WriteJsValue(value.Name);
            serializer.Serialize(writer, value.Options);
            writer.WriteEndJsFunctionSignature();
            writer.WriteEndJsFunctionDefinition();
        }
    }
}