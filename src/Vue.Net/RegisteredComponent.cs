using System;

namespace Vue.Net
{
    public class RegisteredComponent
    {
        public VueComponentOptions Options { get; }

        public Type ModelType { get; }

        public RegisteredComponent(
            Type modelType,
            VueComponentOptions options)
        {
            ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (!ModelType.IsSubclassOf(typeof(VueModel)))
            {
                throw new ArgumentException($"Model inherit from {typeof(VueModel)}", nameof(modelType));
            }
        }
    }
}
