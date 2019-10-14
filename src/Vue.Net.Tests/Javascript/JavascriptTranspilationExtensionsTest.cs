using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vue.Net.Javascript;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript
{
    public class JavascriptTranspilationExtensionsTest
    {
        // public class { }
        public static ClassDeclarationSyntax TestClass =
            SyntaxFactory.ClassDeclaration("class")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        // public int count { get; set; }
        public static PropertyDeclarationSyntax TestAutomaticProperty =
            SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "count")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

        //[Fact]
        //public void TestToJsExpressionWithAnonymousMethodExpressionSyntax()
        //{
        //    var expression = SyntaxFactory.AnonymousMethodExpression();
        //    var jsExpression = expression.ToJsFunctionExpression();

        //    Assert.NotNull(jsExpression);
        //    Assert.Empty(jsExpression.ArgumentNames);
        //    Assert.NotNull(jsExpression.Body);
        //    Assert.Empty(jsExpression.Body.Statements);

        //    expression = SyntaxFactory.AnonymousMethodExpression(
        //        SyntaxFactory.ParameterList(
        //            SyntaxFactory.SeparatedList(new ParameterSyntax[] { SyntaxFactory.Parameter(SyntaxFactory.Identifier("arg")) })),
        //        SyntaxFactory.Block());

        //    jsExpression = expression.ToJsFunctionExpression();
        //    Assert.NotNull(jsExpression);
        //    var identifier = Assert.Single(jsExpression.ArgumentNames);
        //    Assert.Equal("arg", identifier.Token.ValueText);
        //    Assert.NotNull(jsExpression.Body);
        //    Assert.Empty(jsExpression.Body.Statements);
        //}

        //[Fact]
        //public void TestToJsExpressionWithParenthesizedLambdaExpressionSyntax()
        //{
        //    var d = new { d = 3 };
        //}

        //[Fact]
        //public void TestClassDeclarationToJsObject()
        //{
        //    var syntax = TestClass.ToJsObject();

        //    Assert.NotNull(syntax);
        //    Assert.NotNull(syntax.Properties);
        //    Assert.Empty(syntax.Properties);
        //}

        //[Fact]
        //public void TestClassDeclarationToJsObjectWithAutomaticProperty()
        //{
        //    var syntax = TestClass
        //        .AddMembers(TestAutomaticProperty)
        //        .ToJsObject();

        //    Assert.NotNull(syntax);
        //    Assert.Single(syntax.Properties, item =>
        //    {
        //        return item is JsObjectPropertySyntax method && method.Identifier.ValueText == "count";
        //    });
        //}

    }
}
