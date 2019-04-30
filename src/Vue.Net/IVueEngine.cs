using System;

namespace Vue.Net
{
    public interface IVueEngine
    {
        event EventHandler<RegisteredComponent> ComponentUsed;

        RegisteredComponent AddComponent(Type classType, VueComponentOptions options);

        RegisteredComponent AddComponent<T>(VueComponentOptions options) where T : VueModel;

        RegisteredComponent GetComponent(string componentName);

        void NotifyUsed(RegisteredComponent component);
    }
}
