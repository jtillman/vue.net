using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace TesterApp
{
    public class SymbolToStringIdentityCompiler : ISymbolCompiler<string>
    {
        public string Compile(ISymbol symbol) => symbol switch
        {
            IMethodSymbol methodSymbol => Compile(methodSymbol.MethodKind, methodSymbol.Name, methodSymbol.Parameters.Select(parameter=> Compile(parameter)).ToArray()),
            IParameterSymbol parameterSymbol => parameterSymbol.Name,
            IPropertySymbol propertySymbol => Compile(MethodKind.PropertyGet, propertySymbol.Name, Array.Empty<string>()),
            IFieldSymbol fieldSymbol => $"$data${fieldSymbol.Name}$",
            ILocalSymbol localSymbol => localSymbol.Name,
            ITypeSymbol typeSymbol => typeSymbol.ToDisplayString(),
            INamespaceSymbol nsSymbol => nsSymbol.Name,
            _ => throw new ArgumentOutOfRangeException($"Unknown Identifier Type: {symbol} of type {symbol.GetType().Name}")
        };

        public string Compile(MethodKind methodKind, string name, string[] parameters)
        {
            var prefix = methodKind switch
            {
                MethodKind.Constructor => "ctr",
                MethodKind.Ordinary => "fn",
                MethodKind.PropertyGet => "get",
                MethodKind.PropertySet => "set",
                _ => methodKind.ToString()
            };
            return string.Format("${0}${1}{2}$", prefix, name, string.Join("$", parameters));
        }
    }
}
