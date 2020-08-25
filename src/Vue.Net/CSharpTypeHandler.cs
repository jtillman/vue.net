using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;

namespace Vue.Net
{
    public class CSharpTypeHandler
    {
        public Dictionary<string, Func<SyntaxNode, SyntaxNode>> Overrides { get; }
            = new Dictionary<string, Func<SyntaxNode, SyntaxNode>>();

        public CSharpTypeHandler()
        {
            // AliasMethodCall<HttpClient>(c => new HttpClient(), "axios.create");
            // Action<MethodInfo> d = new Action<MethodInfo>((method) => { });
            // d(typeof(string).GetMethod(nameof(string.ToString), Array.Empty<Type>()));
            // AliasMethodCall<string>(s => s.ToString(), "");
            AliasMethodCall<object>(o => o.ToString(), "toString");
            AliasMethodCall<ICollection<string>>(f => f.Add(default), "push");
            AliasMethodCall<HttpClient>(c => c.GetStringAsync(string.Empty), "get");
        }

        private void SetMemberHandler(string memberName, Func<SyntaxNode, SyntaxNode> handler)
        {
            if (Overrides.ContainsKey(memberName))
            {
                throw new ArgumentException("MemberName alread handled");
            }
            Overrides[memberName] = handler;
        }


        private void AliasMethod(MethodInfo method, string alias, Type targetType = null)
        {
            SetMemberHandler(
                GetNameForMethod(method, targetType),
                new Func<SyntaxNode, SyntaxNode>((node)=> {
                    var invocation = node as InvocationExpressionSyntax;
                    return method.IsStatic ? 
                        invocation.WithExpression(SyntaxFactory.IdentifierName(alias)) :
                        (invocation).WithExpression((invocation.Expression as MemberAccessExpressionSyntax).WithName(SyntaxFactory.IdentifierName(alias)));
                    }));
        }

        public void AliasMethodCall<T>(Expression<Action<T>> memberExpression, string alias, Type targetType = null)
        {
            var method = (memberExpression?.Body as MethodCallExpression)?.Method;
            if (null == method)
            {
                throw new ArgumentException("Expression must be a method call");
            }

            AliasMethod(method, alias, targetType);
        }

        public SyntaxNode HandleMember(string memberFullName, SyntaxNode node)
        {
            if (!Overrides.TryGetValue(memberFullName, out var handler))
            {
                return null;
            }
            return handler(node);
        }

        public static string GetNameForMethod(MethodInfo methodInfo, Type targetType = null)
        {
            var declaringType = methodInfo.DeclaringType;

            var className = declaringType.FullName;
            var typeArguments = string.Empty;
            if (declaringType.IsGenericType)
            {
                declaringType = declaringType.GetGenericTypeDefinition();
                className = $"{declaringType.FullName.Substring(0, declaringType.FullName.IndexOf("`"))}<{string.Join(", ", declaringType.GetGenericArguments().Select(arg => arg.Name))}>";
                methodInfo = declaringType.GetMethods().Where(m => m.Name == methodInfo.Name).First();
            }

            return $"{className}:{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(p=>p.ParameterType.FullName??p.ParameterType.Name))})";
        }
    }
}
