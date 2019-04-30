using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vue.Net
{
    [JsonConverter(typeof(VueOptionsConverter))]
    public interface IVueOptions
    {
        string El { get; }

        string Template { get; }

        IList<string> Mixins { get; }

        IList<VueData> Data { get; }

        IList<VueProp> Props { get; }

        IList<VueComputed> Computed { get; }

        IList<VueMethod> Methods { get; }

        IList<VueMethod> LifecycleHooks { get; }
    }
}
