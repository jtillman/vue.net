using System;

namespace Vue.Net.Javascript
{
    public class JsTranspilationException<CSharpType, JsType> : Exception
    {
        public JsTranspilationException(CSharpType value)
            : base($"Cannot convert value {value} to type {typeof(JsType)}")
        {
        }

        public JsTranspilationException(string message) : base ($"Cannot convert ${typeof(CSharpType)} to {typeof(JsType)}")
        {
        }
    }
}
