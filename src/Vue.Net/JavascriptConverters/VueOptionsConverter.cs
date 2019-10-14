using Newtonsoft.Json;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueOptionsConverter : JavascriptConverter<IVueOptions>
    {
        public override void WriteJavascript(
            OldJavascriptTextWriter writer, 
            IVueOptions value,
            JsonSerializer serializer)
        {
            writer.WriteStartJsObject();

            if (null != value.El)
            {
                writer.WriteStartJsMember("el");
                writer.WriteJsValue(value.El);
                writer.WriteEndJsMember();
            }

            if (null != value.Template)
            {
                writer.WriteStartJsMember("template");
                writer.WriteJsValue(value.Template);
                writer.WriteEndJsMember();
            }

            if (value.Data?.Count > 0)
            {
                writer.WriteStartJsMember("data");

                writer.WriteStartJsFunctionDefinition();
                writer.WriteStartJsFunctionSignature("function");

                writer.WriteEndJsFunctionSignature();
                writer.WriteStartJsFunctionBlock();

                writer.WriteStartJsReturnStatement();
                writer.WriteStartJsObject();

                foreach (var data in value.Data)
                {
                    serializer.Serialize(writer, data);
                }

                writer.WriteEndJsObject();
                writer.WriteEndJsStatement();

                writer.WriteEndJsFunctionBlock();
                writer.WriteEndJsFunctionDefinition();

                writer.WriteEndJsMember();
            }

            if (value.Props?.Count > 0)
            {
                writer.WriteStartJsMember("props");

                writer.WriteStartJsObject();
                foreach (var prop in value.Props)
                {
                    serializer.Serialize(writer, prop);
                }
                writer.WriteEndJsObject();
                writer.WriteEndJsMember();
            }

            if (value.Methods?.Count > 0)
            {
                writer.WriteStartJsMember("methods");
                writer.WriteStartJsObject();
                foreach(var method in value.Methods)
                {
                    serializer.Serialize(writer, method);
                }
                writer.WriteEndJsObject();
                writer.WriteEndJsMember();
            }

            if (value.Computed?.Count > 0)
            {
                writer.WriteStartJsMember("computed");
                writer.WriteStartJsObject();
                foreach (var property in value.Computed)
                {
                    serializer.Serialize(writer, property);
                }
                writer.WriteEndJsObject();
                writer.WriteEndJsMember();
            }


            if (value.LifecycleHooks?.Count > 0)
            {
                foreach (var method in value.LifecycleHooks)
                {
                    serializer.Serialize(writer, method);
                }
            }

            writer.WriteEndJsObject();
        }
    }
}
