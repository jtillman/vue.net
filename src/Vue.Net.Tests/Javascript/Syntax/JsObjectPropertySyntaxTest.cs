using Vue.Net.Javascript.Syntax;
using Xunit;


namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsObjectPropertySyntaxTest
    {
        [Fact]
        public void TestConstructorSetsProperties()
        {
            var identifier = JsSyntaxToken.Identifier("prop1");
            var value = JsExpressionSyntax.Null;

            var syntax = new JsObjectPropertySyntax(identifier, value);

            Assert.Equal(identifier, syntax.Identifier);
            Assert.Equal(value, syntax.Value);
        }

        [Fact]
        public void TestWritesCorrectly()
        {
            var identifier = JsSyntaxToken.Identifier("prop1");

            var value = JsExpressionSyntax.Null;

            var syntax = new JsObjectPropertySyntax(identifier, value);

            syntax.AssertWrites("prop1: null");
        }
    }
}
