using Newtonsoft.Json;

namespace Vue.Net.Javascript
{
    public class JavascriptSerializer : JsonSerializer
    {
        public JavascriptSerializer()
        {
            Converters.Add(new CSharpSyntaxConverter());
        }
    }
}
