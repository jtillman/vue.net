using Microsoft.CodeAnalysis;

namespace TesterApp
{
    public interface ISymbolCompiler<T> : ICompiler<ISymbol, T>
    {
    }
}
