using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vue.Net.Javascript
{
    public class JavascriptPackage
    {
        private CSharpCompilation _compilation;

        public List<Assembly> _referencedAssemblies = new List<Assembly>();

        public Dictionary<Assembly, CSharpDecompiler> _decompilers = new Dictionary<Assembly, CSharpDecompiler>();

        public DecompilerSettings _decompilerSettings = new DecompilerSettings { ThrowOnAssemblyResolveErrors = false };

        public JavascriptPackage(string assemblyName)
        {
            _compilation = CSharpCompilation.Create(assemblyName);
            EnsureReferenced(typeof(object).Assembly);
        }

        private void EnsureReferenced(Assembly assembly)
        {
            var assemblies = assembly.GetReferencedAssemblies().Select(assemblyName => Assembly.Load(assemblyName)).Prepend(assembly);
            var newAssemblies = assemblies.Where(a => !_referencedAssemblies.Contains(a)).ToArray();
            if (newAssemblies.Any())
            {
                _compilation = _compilation.AddReferences(newAssemblies.Select(a => MetadataReference.CreateFromFile(a.Location)));
                _referencedAssemblies.AddRange(newAssemblies);
            }
        }

        public string Decompile(Type csharpType)
        {
            var assembly = csharpType.Assembly;
            if (!_decompilers.TryGetValue(assembly, out var decompiler))
            {
                decompiler = new CSharpDecompiler(assembly.Location, _decompilerSettings);
                _decompilers[assembly] = decompiler;
            }
            return decompiler.DecompileTypeAsString(new FullTypeName(csharpType.FullName));
        }

        public void AddType<T>() => AddType(typeof(T));

        public void AddType(Type csharpType)
        {
            var typeText = Decompile(csharpType);
            var syntaxTree = CSharpSyntaxTree.ParseText(typeText);

            EnsureReferenced(csharpType.Assembly);

            var compilation = _compilation.AddSyntaxTrees(syntaxTree);
            var classDeclaration = syntaxTree.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cls => cls.Identifier.ValueText == csharpType.Name)
                .First();

            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            var referencedTypes = classDeclaration
                .DescendantNodes()
                .OfType<TypeSyntax>()
                .Select(nameSyntax => semanticModel.GetSymbolInfo(nameSyntax).Symbol)
                .OfType<ITypeSymbol>()
                .Distinct()
                .ToArray();

            var transpiler = new CSharpToJavascriptTranspiler(semanticModel);
            var obj = transpiler.ToJsObjectCreationExpression(classDeclaration);
            var syntaxRewriter = new VueSyntaxRewriter(semanticModel);
            syntaxRewriter.Visit(classDeclaration);

            var writer = new StringWriter();
            obj.WriteTo(writer);
            var result = writer.ToString();

            var classTypeInfo = semanticModel.GetTypeInfo(classDeclaration);
            var classSymbol = semanticModel.GetSymbolInfo(classDeclaration);
        }
    }
}
