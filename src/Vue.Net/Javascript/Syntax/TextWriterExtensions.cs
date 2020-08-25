using System.IO;
using System.Linq;

namespace Vue.Net.Javascript.Syntax
{
    public static class TextWriterExtensions
    {
        public static TextWriter WriteJsSyntax(this TextWriter textWriter, params JsSyntax[] jsSyntaxNodes) {
            if (null == jsSyntaxNodes) return textWriter;

            foreach (var jsSyntax in jsSyntaxNodes.Where(node=>null != node))
            {
                jsSyntax.WriteTo(textWriter);
            }
            return textWriter;
        }
    }
}
