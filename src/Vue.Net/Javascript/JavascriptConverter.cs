using System;
using Newtonsoft.Json;

namespace Vue.Net.Javascript
{
    public abstract class JavascriptConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.Equals(typeof(T))|| objectType.IsSubclassOf(typeof(T));

        public override object ReadJson(
            JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
            => throw new NotSupportedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsWriter = writer as JavascriptTextWriter;
            if (null == jsWriter)
            {
                throw new ArgumentException("Writer object must be a JavascriptWriter");
            }

            if (!(value is T))
            {
                throw new ArgumentException($"Value of type { value?.GetType() } is not of (sub)type { typeof(T) }");
            }
            WriteJavascript(jsWriter, (T) value, serializer);
        }

        public abstract void WriteJavascript(JavascriptTextWriter writer, T value, JsonSerializer serializer);
    }
}
