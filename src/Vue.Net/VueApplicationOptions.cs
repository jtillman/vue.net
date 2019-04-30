using System;
using System.Collections.Generic;
using System.Text;
using Vue.Net.Javascript;

namespace Vue.Net
{
    public class VueGlobalOptions
    {
        public IList<RegisteredComponent> Components { get; }
            = new List<RegisteredComponent>();

        public JavascriptSerializer Serializer { get; set; }
            = new JavascriptSerializer();

        public CSharpTypeHandler TypeHandler { get; set; }
            = new CSharpTypeHandler();

        public  void RegisterComponent<TVueType>(VueComponentOptions componentOptions = null) where TVueType : VueModel
        {
            componentOptions = componentOptions ?? new VueComponentOptions();
            componentOptions.Name = componentOptions.Name ?? typeof(TVueType).Name;
            Components.Add(new RegisteredComponent(typeof(TVueType), componentOptions));
        }
    }
}
