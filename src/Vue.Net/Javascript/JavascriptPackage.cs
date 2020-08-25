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
using System.Security.Cryptography.Xml;

namespace Vue.Net.Javascript
{
    public class JavascriptPackage
    {
        public Dictionary<string, JsType> TypeDefinitions { get; } =
            new Dictionary<string, JsType>();

        public void AddType(JsType jsType)
        {
            if (TypeDefinitions.ContainsKey(jsType.Name))
            {
                throw new ArgumentException($"Duplicate type name: {jsType.Name}");
            }

            TypeDefinitions[jsType.Name] = jsType;

            //using (var writer = new StringWriter())
            //{
            //    obj.WriteTo(writer);
            //    var result = writer.ToString();
            //}

            //var classTypeInfo = semanticModel.GetTypeInfo(classDeclaration);
            //var classSymbol = semanticModel.GetSymbolInfo(classDeclaration);
        }
    }


    public class CSharpToJsTranspilerEnvironment
    {
        private CSharpCompilation _compilation;

        private List<Assembly> _referencedAssemblies = new List<Assembly>();

        private Dictionary<Assembly, CSharpDecompiler> _decompilers = new Dictionary<Assembly, CSharpDecompiler>();

        private DecompilerSettings _decompilerSettings = new DecompilerSettings { ThrowOnAssemblyResolveErrors = false };

        public CSharpToJsTranspilerEnvironment(string assemblyName)
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

        private string Decompile(Type csharpType)
        {
            var assembly = csharpType.Assembly;
            if (!_decompilers.TryGetValue(assembly, out var decompiler))
            {
                decompiler = new CSharpDecompiler(assembly.Location, _decompilerSettings);
                _decompilers[assembly] = decompiler;
            }
            return decompiler.DecompileTypeAsString(new FullTypeName(csharpType.FullName));
        }

        public JsType TranspileType(Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));

            var typeText = Decompile(type);
            var syntaxTree = CSharpSyntaxTree.ParseText(typeText);

            EnsureReferenced(type.Assembly);

            var compilation = _compilation.AddSyntaxTrees(syntaxTree);
            var classDeclaration = syntaxTree.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cls => cls.Identifier.ValueText == type.Name)
                .First();

            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            var referenceWalker = new CSharpReferenceWalker(semanticModel);
            referenceWalker.Visit(syntaxTree.GetRoot());
            var references = referenceWalker.References.ToArray();

            var csharpType = new CSharpType(classDeclaration, references);

            return new JsType(type.FullName, semanticModel);

            var referencedTypes = classDeclaration
                .DescendantNodes()
                .OfType<TypeSyntax>()
                .Select(nameSyntax => semanticModel.GetSymbolInfo(nameSyntax).Symbol)
                .OfType<ITypeSymbol>()
                .Distinct()
                .ToArray();

            var transpiler = new CSharpToJavascriptTranspiler(semanticModel);
            var obj = transpiler.ToJsObjectCreationExpression(classDeclaration);

            return null;
        }
    }

    public class JsTypeMember
    {
        public string Name { get; }

        public JsTypeMember(string name)
        {
            Name = name;
        }
    }

    public class JsFieldMember : JsTypeMember
    {
        public JsFieldMember(string name) : base(name) { }
    }

    public class JsType : JsTypeMember
    {
        public SemanticModel SemanticModel { get; }

        public JsTypeMember[] Members { get; set; }

        public JsType(string name, SemanticModel semanticModel) : base(name)
        {
            SemanticModel = semanticModel;
        }
    }

    public class CSharpMemberInfo
    {
        public CSharpTypeInfo Type { get; set; }

        public string Name { get; set; }

        public CSharpTypeInfo[] Arguments { get; set; }
    }

    public class CSharpTypeInfo
    {
        public string Module { get; set; }

        public string Name { get; set; }
    }

    public class CSharpReferenceInfo
    {
        public ExpressionSyntax Expression { get; set; }

        public CSharpMemberInfo MemberInfo { get; set; }
    }

    public class CSharpReferenceWalker : CSharpSyntaxWalker
    {
        public List<CSharpReferenceInfo> References { get; }
            = new List<CSharpReferenceInfo>();

        public SemanticModel SemanticModel { get; }

        public CSharpReferenceWalker(SemanticModel semanticModel)
        {
            SemanticModel = semanticModel;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            AddReference(node);
            base.VisitInvocationExpression(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            AddReference(node);
            base.VisitMemberAccessExpression(node);
        }

        private void AddReference(ExpressionSyntax node)
        {
            var symbolInfo = this.SemanticModel.GetSymbolInfo(node);
            var symbol = symbolInfo.Symbol;

            if (null != symbol)
            {
                var reference = new CSharpReferenceInfo
                {
                    MemberInfo = ToMemberInfo(symbol),
                    Expression = node
                };

                this.References.Add(reference);
            }
        }

        private static CSharpMemberInfo ToMemberInfo(Microsoft.CodeAnalysis.ISymbol symbol)
        {
            var parameters = Array.Empty<CSharpTypeInfo>();
            if (symbol.Kind == Microsoft.CodeAnalysis.SymbolKind.Method)
            {
                parameters = (symbol as IMethodSymbol).Parameters.Select(p => {
                    return new CSharpTypeInfo
                    {
                        Module = p.Type.ContainingModule.ToString(),
                        Name = p.Type.Name
                    };
                }).ToArray();
            }

            var memberInfo = new CSharpMemberInfo
            {
                Type = new CSharpTypeInfo
                {
                    Module = symbol.ContainingModule.ToString(),
                    Name = symbol.ContainingType.ToString()
                },
                Name = symbol.Name,
                Arguments = parameters
            };

            return memberInfo;
        }
    }

    public class CSharpType
    {
        public ClassDeclarationSyntax ClassDeclaration { get; }

        public Dictionary<ExpressionSyntax, CSharpReferenceInfo> References { get; }

        public CSharpType(
            ClassDeclarationSyntax classDeclaration,
            CSharpReferenceInfo[] references)
        {
            ClassDeclaration = classDeclaration;
            References = references.ToDictionary(reference => reference.Expression);
        }
    }

    public class JsCSharpInterpreter
    {
    }
}
