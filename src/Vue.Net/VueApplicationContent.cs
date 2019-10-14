using System.Collections.Generic;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueApplicationContent : IJavascriptContent
    {
        private readonly string _name;
        private readonly JavascriptSerializer _serializer;
        private readonly IList<IVueComponent> _components;
        private readonly VueOptions _options;

        public VueApplicationContent(
            JavascriptSerializer serializer,
            string name,
            VueOptions options,
            IList<IVueComponent> components)
        {
            _name = name;
            _serializer = serializer;
            _options = options;
            _components = components;
        }

        public void WriterTo(OldJavascriptTextWriter writer)
        {
            // Opening an async closure to run script
            writer.WriteStartJsFunctionSignature(string.Empty);
            writer.WriteStartJsFunctionDefinition();
            writer.WriteStartJsFunctionSignature("function", "async");
            writer.WriteEndJsFunctionSignature();
            writer.WriteStartJsFunctionBlock();

            // Statements to register Vue Components
            foreach (var component in _components)
            {
                writer.WriteStartJsStatement();
                _serializer.Serialize(writer, component);
                writer.WriteEndJsStatement();
            }

            // Statement to create Vue Application
            writer.WriteStartJsVariableStatement(_name);
            writer.WriteStartJsFunctionDefinition();
            writer.WriteStartJsFunctionSignature("Vue", "new");
            _serializer.Serialize(writer, _options);
            writer.WriteEndJsFunctionSignature();
            writer.WriteEndJsFunctionDefinition();
            writer.WriteEndJsStatement();

            // Closing Async Closure and invoking
            writer.WriteEndJsFunctionBlock();
            writer.WriteEndJsFunctionDefinition();
            writer.WriteEndJsFunctionSignature();
            writer.WriteStartJsFunctionSignature(string.Empty);
            writer.WriteEndJsFunctionSignature();
        }
    }
}
