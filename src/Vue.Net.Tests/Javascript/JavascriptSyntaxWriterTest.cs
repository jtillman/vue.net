using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using Vue.Net.Javascript;
using Xunit;

namespace Vue.Net.Tests.Javascript
{
    public class JavascriptSyntaxWriterTest
    {
        private void TestJsEquivalence<T>(string csharpSyntax, string expectedJs) where T : SyntaxNode
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(csharpSyntax);

            using (var txtWriter = new StringWriter())
            {
                var syntaxWriter = new JavascriptSyntaxWriter(new OldJavascriptTextWriter(txtWriter));
                var node = syntaxTree.GetRoot().DescendantNodes().OfType<T>().Single();
                syntaxWriter.Visit(node);
                Assert.Equal(
                    expectedJs,
                    txtWriter.ToString());
            }
        }

        [Fact]
        public void TestConvertsClass() =>
            TestJsEquivalence<ClassDeclarationSyntax>(
                "class A { }",
                "A = function() { return { }; }");

        [Fact]
        public void TestConvertClassIntProperty() =>
            TestJsEquivalence<ClassDeclarationSyntax>(
                "class A { public int B { get; set;} }",
                "A = function() { return { B: 0}; }");

        [Fact]
        public void TestConvertClassIntPropertyDefault() =>
            TestJsEquivalence<ClassDeclarationSyntax>(
                "class A { public int B { get; set; } = 1; }",
                "A = function() { return { B: 1}; }");

        [Fact]
        public void TestConvertClassStringProperty() =>
            TestJsEquivalence<ClassDeclarationSyntax>(
                "class A { public string B { get; set; } }",
                "A = function() { return { B: null }; }");

        [Fact]
        public void TestConvertClassStringPropertyDefault() =>
            TestJsEquivalence<ClassDeclarationSyntax>(
                "class A { public string B { get; set; } = \"default\"; }",
                "A = function() { return { B: \"default\" }; }");

        [Fact]
        public void TestConvertClass() { }

    }
}
