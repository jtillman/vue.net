using System;
using System.Runtime.InteropServices.ComTypes;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{
    public interface ITypeDecompiler
    {
        string DecompileTypeAsString(Type type);
        string DecompileTypeAsString<T>();
    }
}
