using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Vue.Net
{
    [JsonConverter(typeof(VueMethodConverter))]
    public class VueMethod
    {
        public string Name { get; set; }

        public IList<string> ParameterNames { get; set; }

        public MethodDeclarationSyntax Declaration { get; set; }
    }
}
