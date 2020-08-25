using System.IO;

namespace Vue.Net.Javascript.Syntax
{
    public sealed class JsEmptySyntax : JsSyntax
    {
        public static JsEmptySyntax Instance { get; } = new JsEmptySyntax();

        private JsEmptySyntax() { }

        public override void WriteTo(TextWriter textWriter) { }
    }
}
