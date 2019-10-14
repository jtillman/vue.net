using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vue.Net.Javascript;
using Vue.Net.Javascript.Syntax;

namespace Vue.Net
{
    public class CSharpJsCompiler
    {
        public CSharpDecompiler Decompiler { get; }

        public CSharpCompilation Compilation { get; }

        public Dictionary<string, CSharpDecompiler> Decompilers { get; }
            = new Dictionary<string, CSharpDecompiler>();

        public Assembly ApplicationAssembly { get; }

        public CSharpJsCompiler(Assembly applicationAssembly)
        {
            ApplicationAssembly = applicationAssembly;

            Compilation = CSharpCompilation.Create("VueNet").AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(ApplicationAssembly.Location))
                .AddReferences(ApplicationAssembly.GetReferencedAssemblies().Select(assembly => MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)));
        }

        public CSharpDecompiler GetDecompilerForType<T>()
        {
            var location = typeof(T).Assembly.Location;
            if (!Decompilers.TryGetValue(location, out var decompiler))
            {
                decompiler = new CSharpDecompiler(typeof(T).Assembly.Location, new DecompilerSettings()
                {
                    ThrowOnAssemblyResolveErrors = false
                });
                Decompilers[location] = decompiler;
            }
            return decompiler;
        }

        public JsSyntax GetJsDefinitionCompilation<T>()
        {
            var codeType = typeof(T);
            var decompiler = GetDecompilerForType<T>();
            var typeText = decompiler.DecompileTypeAsString(new FullTypeName(codeType.FullName));
            var syntaxTree = CSharpSyntaxTree.ParseText(typeText);

            var compilation = Compilation.AddSyntaxTrees(syntaxTree);

            var types = syntaxTree.GetRoot().DescendantNodes()
                .OfType<TypeSyntax>()
                .Select(t => t.ToString()).ToArray();

            var type = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cls => cls.Identifier.ValueText == codeType.Name)
                .First();

            var model = compilation.GetSemanticModel(syntaxTree);

            var memberCalls = type.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>().ToArray();

            var d = new Dictionary<string, string>();
            //var b = (string s) => new JsObjectCreationExpressionSyntax(new JsBaseObjectPropertySyntax[0]);


            // Rewrite current
            foreach (var call in memberCalls)
            {
                var symbolInfo = model.GetSymbolInfo(call);
                var symbol = symbolInfo.Symbol;
            }
            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);
            var transpiler = new CSharpToJavascriptTranspiler(semanticModel);
            return transpiler.ToJsObjectCreationExpression(type);
        }
    }

    // Replace all calls to string.blah to something else

    //public class JsTypeDefinition
    //{
    //    public Dictionary<string, JsSyntax> TypeMembers { get; }

    //    public JsObjectPropertySyntax Length { get; }
    //        = new JsMemberAccessorExpressionSyntax();

    //    public int Length {
    //        get
    //        {
    //            return (int)((dynamic)this).length;
    //        }
    //    }
    //}

    public class JsAnglesType
    {
        public void Write()
        {
            // TranspileProperty<string>(str=>str.Length, 
            //   JsSyntax.Property("length")
            // TranspilerMethod<method>
            // Over
        }
    }

    public class CompiledVueModelComponent : IVueComponent 
    {
        private string _name;
        public string Name
        {
            get
            {
                if (null == _name)
                {
                    _name = _componentOptions.Name ?? GetComponentName();
                }
                return _name;
            }
        }

        private IVueOptions _options;
        public IVueOptions Options
        {
            get
            {
                if (null == _options)
                {
                    _options = GetComponentVueOptions();
                }
                return _options;
            }
        }

        public Type SystemType { get; }

        private CSharpDecompiler _decompiler;
        public CSharpDecompiler Decompiler
        {
            get
            {
                if (null == _decompiler)
                {
                    _decompiler = new CSharpDecompiler(
                        SystemType.Assembly.Location,
                        new DecompilerSettings()
                        {
                            ThrowOnAssemblyResolveErrors = false
                        });
                }
                return _decompiler;
            }
        }

        private IType _decompiledType;
        public IType DecompiledType
        {
            get
            {
                if (null == _decompiledType)
                {
                    _decompiledType = Decompiler.TypeSystem.MainModule.Compilation.FindType(new FullTypeName(SystemType.FullName));
                }
                return _decompiledType;
            }
        }

        private ClassDeclarationSyntax _declarationSyntax;
        public ClassDeclarationSyntax DeclarationSyntax {
            get
            {
                if (null == _declarationSyntax)
                {
                    var typeText = Decompiler.DecompileTypeAsString(new FullTypeName(SystemType.FullName));
                    var syntaxTree = CSharpSyntaxTree.ParseText(typeText);

                    var compilation = CSharpCompilation.Create("VueNet")
                        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                        .AddReferences(MetadataReference.CreateFromFile(SystemType.Assembly.Location))
                        .AddReferences(SystemType.Assembly.GetReferencedAssemblies().Select(a=>MetadataReference.CreateFromFile(Assembly.Load(a).Location)))
                        .AddSyntaxTrees(syntaxTree);

                    var rewriter = new VueSyntaxRewriter(compilation.GetSemanticModel(syntaxTree), _typeHandler);
                    _declarationSyntax = rewriter.Visit(syntaxTree.GetRoot()).DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

                }
                return _declarationSyntax;
            }
        }

        private readonly object _instance;

        private readonly VueComponentOptions _componentOptions;

        private readonly CSharpTypeHandler _typeHandler;

        public CompiledVueModelComponent(
            Type systemType,
            object instance,
            VueComponentOptions componentOptions,
            CSharpTypeHandler typeHandler = null)
        {
            SystemType = systemType;
            _instance = instance;
            _componentOptions = componentOptions;
            _typeHandler = typeHandler;
        }

        internal string GetComponentName()
        {
            return DecompiledType.Name;
        }

        internal IVueOptions GetComponentVueOptions()
        {
            var options = new VueOptions()
            {
                Template = _componentOptions.Template
            };

            options.Data = GetComponentData();
            options.Computed = GetComponentComputed();
            options.Methods = GetComponentMethods();
            options.LifecycleHooks = GetComponentLifecycleHooks();
            return options;
        }

        internal IList<VueData> GetComponentData()
        {
            var dataList = new List<VueData>();

            foreach(var field in DecompiledType.GetFields(f=>f.Accessibility.HasFlag(ICSharpCode.Decompiler.TypeSystem.Accessibility.Public) && !f.IsStatic))
            {
                dataList.Add(ToVueField(field));
            }

            var fieldProps = DeclarationSyntax
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => !p.AccessorList.Accessors.Any(accessor => accessor.Body != null));

            foreach( var prop in fieldProps)
            {
                dataList.Add(new VueData
                {
                    Name = prop.Identifier.ValueText,
                    Value = SystemType.GetProperty(prop.Identifier.ValueText).GetValue(_instance)
                });
            }

            return dataList;
        }

        internal IList<VueMethod> GetComponentMethods()
        {
            var methods = new List<VueMethod>();
            foreach(var method in DecompiledType.GetMethods(m=> m.DeclaringType.DirectBaseTypes.Any(c=>c.FullName == typeof(VueModel).FullName)))
            {
                methods.Add(ToVueMethod(method));
            }
            return methods;
        }

        internal IList<VueMethod> GetComponentLifecycleHooks()
        {
            var methods = new List<VueMethod>();
            foreach(var method in DecompiledType.GetMethods())
            {
                if (!VueModel.LifeCycleMethodsNameMapping.TryGetValue(method.Name, out var hookName))
                {
                    continue;
                }

                if (method.DeclaringType.Name == nameof(VueModel)) {
                    continue;
                }
                var vueMethod = ToVueMethod(method);
                vueMethod.Name = hookName;
                methods.Add(vueMethod);
            }
            return methods;
        }
        internal IList<VueComputed> GetComponentComputed()
        {
            var computed = new List<VueComputed>();
            foreach(var property in DecompiledType.GetProperties())
            {
                var compilation = GetSyntaxTree(property);
                if (compilation.Members.First() is PropertyDeclarationSyntax prop){
                    if (!prop.AccessorList.Accessors.Any(accessor=>accessor.Body != null))
                    {
                        continue;
                    }
                }
                computed.Add(new VueComputed
                {
                    Name = property.Name,
                    Body = compilation
                });
            }
            return computed;
        }

        internal VueData ToVueField(IField field)
        {
            var fieldInfo = SystemType.GetField(field.Name);
            return new VueData
            {
                Name = field.Name,
                Value = fieldInfo.GetValue(_instance)
            };
        }

        internal VueData ToVueField(IProperty property)
        {
            var propertyInfo = SystemType.GetProperty(property.Name);
            return new VueData
            {
                Name = property.Name,
                Value = propertyInfo.GetValue(_instance)
            };
        }

        internal VueMethod ToVueMethod(IMethod method)
        {
            return new VueMethod
            {
                Name = method.Name,
                ParameterNames = method.Parameters.Select(p => p.Name).ToList(),
                Declaration = DeclarationSyntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Single(m=>m.Identifier.ValueText == method.Name)
        };
        }

        internal CompilationUnitSyntax GetSyntaxTree(IMethod method)
        {
            var decompiler = GetOrCreateDecompile(method.ParentModule.PEFile.FileName);
            var methodText = decompiler.DecompileAsString(method.MetadataToken);
            var syntax = CSharpSyntaxTree.ParseText(methodText);
            return syntax.GetCompilationUnitRoot();
        }
       
        internal CompilationUnitSyntax GetSyntaxTree(IProperty property)
        {
            var decompiler = this.GetOrCreateDecompile(property.ParentModule.PEFile.FileName);
            var propertyText = decompiler.DecompileAsString(property.MetadataToken);
            var syntax = CSharpSyntaxTree.ParseText(propertyText, new CSharpParseOptions { });
            return syntax.GetCompilationUnitRoot();
        }

        private Dictionary<string, CSharpDecompiler> _decompilers
            = new Dictionary<string, CSharpDecompiler>();

        internal CSharpDecompiler GetOrCreateDecompile(string  fileName)
        {
            if (!_decompilers.TryGetValue(fileName, out var decompiler))
            {
                decompiler = new CSharpDecompiler(
                    fileName, new DecompilerSettings
                    {
                        ThrowOnAssemblyResolveErrors = false
                    });
                _decompilers[fileName] = decompiler;
            }
            return decompiler;
        }
    }

    public class CompiledVueModelComponent<TVueModel> : CompiledVueModelComponent
    {
        public CompiledVueModelComponent(TVueModel instance, VueComponentOptions componentOptions) : base(typeof(TVueModel), instance, componentOptions)
        {
        }
    }
}
