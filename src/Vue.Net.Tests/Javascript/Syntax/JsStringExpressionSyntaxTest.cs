using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsLiteralExpressionSytaxText : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsValue()
        {
            var token = JsSyntaxToken.Null;
            var syntax = new JsLiteralExpressionSyntax(token);

            Assert.Equal(token, syntax.Token);
        }

        [Theory]
        [InlineData("regular", "\"regular\"")]
        [InlineData("\"quoted\"", "\"\\\"quoted\\\"\"")]
        public void TestWritesJsString(string value, string expected)
        {
            var token = JsSyntaxToken.StringLiteral(value);
            var syntax = new JsLiteralExpressionSyntax(token);

            AssertWrites(expected, syntax);
        }
    }
}
