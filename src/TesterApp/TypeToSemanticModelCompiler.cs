using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vue.Net.Javascript.Syntax;

namespace TesterApp
{


    public class TypeToSemanticModelCompiler : ITypeDecompiler, ICompiler<Type, SemanticModel>
    {
        internal Dictionary<Assembly, CSharpDecompiler> AssemblyDecompilers { get; } =
            new Dictionary<Assembly, CSharpDecompiler>();

        internal static Dictionary<AssemblyName, MetadataReference> AssemblyMetadataReferences { get; } =
            new Dictionary<AssemblyName, MetadataReference>();

        internal static MetadataReference GetAssemblyMetadataReference(AssemblyName assemblyName)
        {
            if (!AssemblyMetadataReferences.TryGetValue(assemblyName, out var metadataReference))
            {
                metadataReference = MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location);
                AssemblyMetadataReferences[assemblyName] = metadataReference;
            }
            return metadataReference;
        }


        internal Dictionary<Assembly, CSharpCompilation> AssemblyCompilations { get; } =
            new Dictionary<Assembly, CSharpCompilation>();


        internal CSharpCompilation GetCompilationForAssembly(Assembly assembly)
        {
            if (!AssemblyCompilations.TryGetValue(assembly, out var compilation))
            {

                compilation = CSharpCompilation.Create(Guid.NewGuid().ToString())
                    .AddReferences(GetAssemblyMetadataReference(typeof(object).Assembly.GetName()))
                    .AddReferences(GetAssemblyMetadataReference(assembly.GetName()))
                    .AddReferences(assembly.GetReferencedAssemblies().Select(assemblyName => GetAssemblyMetadataReference(assemblyName)));

                AssemblyCompilations[assembly] = compilation;
            }
            return compilation;
        }

        public string DecompileTypeAsString(Type type)
        {
            var assembly = type.Assembly;
            var decompiler = AssemblyDecompilers.GetOrCreate(assembly, () => new CSharpDecompiler(assembly.Location, new ICSharpCode.Decompiler.DecompilerSettings()));
            return decompiler.DecompileTypeAsString(new FullTypeName(type.FullName));
        }

        public string DecompileTypeAsString<T>() => DecompileTypeAsString(typeof(T));

        public DecompiledType DecompileType(Type type)
        {
            var stringSyntax = DecompileTypeAsString(type);
            var syntaxTree = CSharpSyntaxTree.ParseText(stringSyntax);
            
            var compilation = GetCompilationForAssembly(type.Assembly)
                .AddSyntaxTrees(syntaxTree);
            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);
            return new DecompiledType { Type = type, SemanticModel = semanticModel };
        }

        public DecompiledType DecompileType<T>() => DecompileType(typeof(T));

        public SemanticModel Compile(Type type)
        {
            var stringSyntax = DecompileTypeAsString(type);
            var syntaxTree = CSharpSyntaxTree.ParseText(stringSyntax);

            var compilation = GetCompilationForAssembly(type.Assembly)
                .AddSyntaxTrees(syntaxTree);
            return compilation.GetSemanticModel(syntaxTree, true);
        }
    }
}
