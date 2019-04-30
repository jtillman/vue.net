using System.Collections.Generic;

namespace Vue.Net
{

    public class VueOptions : IVueOptions
    {
        public string El { get; set; }

        public string Template { get; set; }

        public IList<string> Mixins { get; set; }

        public IList<VueData> Data { get; set; }

        public IList<VueProp> Props { get; set; }

        public IList<VueComputed> Computed { get; set; }

        public IList<VueMethod> Methods { get; set; }

        public IList<VueMethod> LifecycleHooks { get; set; }
    }
}
