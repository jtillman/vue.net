using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsArrayCreationExpressionSyntaxTest : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsValues()
        {
            var values = new JsExpressionSyntax[0];
            var syntax = new JsArrayCreationExpressionSyntax(values);

            Assert.Equal(values, syntax.Values);
        }

        [Fact]
        public void TestWritesCorrectlyWithNoValues()
        {
            var values = new JsExpressionSyntax[0];
            var syntax = new JsArrayCreationExpressionSyntax(values);

            AssertWrites("[]", syntax);
        }

        [Fact]
        public void TestWritesCorrectlyWithSingleValue()
        {
            var values = new JsExpressionSyntax[]
            {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("firstValue"))
            };

            var syntax = new JsArrayCreationExpressionSyntax(values);

            AssertWrites("[firstValue]", syntax);
        }

        [Fact]
        public void TestWritesCorrectlyWithMultipleValues()
        {
            var values = new JsExpressionSyntax[]
            {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("firstValue")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("secondValue")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("thirdValue"))
            };

            var syntax = new JsArrayCreationExpressionSyntax(values);

            AssertWrites("[firstValue, secondValue, thirdValue]", syntax);
        }
    }
}
