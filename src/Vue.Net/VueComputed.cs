using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Vue.Net
{
    [JsonConverter(typeof(VueComputedConverter))]
    public class VueComputed
    {
        public string Name { get; set; }

        public CompilationUnitSyntax Body { get;set;}

        public VueMethod Getter { get; set; }

        public VueMethod Setter { get; set; }
    }
}
