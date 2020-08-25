using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsMemberAccessorExpressionSyntaxTest
    {

        [Fact]
        public void TestConstructorSetProperties()
        {
            var expression = new JsIdentifierNameSyntax(
                JsSyntaxToken.Identifier("obj"));
            var name = JsSyntaxToken.Identifier("field");

            var syntax = new JsMemberAccessorExpressionSyntax(
                expression,
                name);

            Assert.Equal(expression, syntax.Expression);
            Assert.Equal(name, syntax.Name);
        }

        [Fact]
        public void TestWriteToWritesCorrectJs()
        {
            var expression = new JsIdentifierNameSyntax(
                JsSyntaxToken.Identifier("obj"));

            var name = JsSyntaxToken.Identifier("field");

            var syntax = new JsMemberAccessorExpressionSyntax(
                expression,
                name);

            syntax.AssertWrites("obj.field");
        }
    }
}
