using Newtonsoft.Json;

namespace Vue.Net
{
    public class VueComponent : IVueComponent
    {
        public string Name { get; set; }

        public IVueOptions Options { get; set; }

        public VueComponent(
            string name, 
            IVueOptions options = null)
        {
            Name = name;
            Options = options ?? new VueOptions();
        }
    }
}
