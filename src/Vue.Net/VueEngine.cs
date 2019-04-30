using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vue.Net
{


    public class VueEngine : IVueEngine
    {
        public event EventHandler<RegisteredComponent> ComponentUsed;

        private readonly ILogger _logger;

        private Dictionary<string, RegisteredComponent> RegisteredComponents { get; }
            = new Dictionary<string, RegisteredComponent>(StringComparer.OrdinalIgnoreCase);


        public VueEngine(
            ILogger<VueEngine> logger,
            IOptions<VueGlobalOptions> options)
        {
            _logger = logger;
            foreach(var component in options.Value.Components)
            {
                AddComponent(component);
            }
        }

        private void AddComponent(RegisteredComponent component)
        {
            var componentName = component.Options.Name ?? component.ModelType.Name;
            if (null != componentName)
            {
                var elementName = new StringBuilder(componentName);
                for ( int i = 0; i < elementName.Length; i++)
                {
                    if (!Char.IsUpper(elementName[i]))
                    {
                        continue;
                    }
                    
                    if (i == 0)
                    {
                        elementName = elementName.Replace(elementName[i], Char.ToLower(elementName[i]), 0, 1);
                        continue;
                    }

                    elementName = elementName.Replace(elementName[i].ToString(), $"-{Char.ToLower(elementName[i])}", i, 1);
                }

                if (RegisteredComponents.ContainsKey(componentName))
                {
                    throw new ArgumentException($"Component with name {componentName} already registered", nameof(component));
                }

                if (RegisteredComponents.ContainsKey(elementName.ToString()))
                {
                    throw new ArgumentException($"Component with name {elementName} already registered", nameof(component));
                }

                RegisteredComponents[componentName] = component;
                RegisteredComponents[elementName.ToString()] = component;
            }

        }

        public RegisteredComponent AddComponent(Type classType, VueComponentOptions options)
        {
            var componentName = options.Name ?? classType.Name;
            var component = new RegisteredComponent(classType, options);
            AddComponent(component);
            return component;
        }

        public RegisteredComponent AddComponent<TVueModel>(VueComponentOptions options = null) where TVueModel : VueModel
        {
            return AddComponent(typeof(TVueModel), options ?? new VueComponentOptions());
        }

        public RegisteredComponent GetComponent(string componentName)
        {
            RegisteredComponents.TryGetValue(componentName, out var component);
            return component;
        }

        public void NotifyUsed(RegisteredComponent component)
        {
            ComponentUsed?.Invoke(this, component);
        }
    }
}
