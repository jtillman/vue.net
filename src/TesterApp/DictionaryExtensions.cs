using System;
using System.Collections.Generic;

namespace TesterApp
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> creator)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = creator();
                dictionary[key] = value;
            }
            return value;
        }
    }
}
