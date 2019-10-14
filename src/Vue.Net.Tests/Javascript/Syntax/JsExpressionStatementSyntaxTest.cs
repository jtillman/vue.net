using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsExpressionStatementSyntaxTest : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsExpressionProperty()
        {
            var expression = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("expression"));
            var syntax = new JsExpressionStatementSyntax(expression);

            Assert.Equal(expression, syntax.Expression);
        }

        [Fact]
        public void TestWritesStatementCorrectly()
        {
            var expression = new JsIdentifierNameSyntax(
                JsSyntaxToken.Identifier("expression"));

            var syntax = new JsExpressionStatementSyntax(expression);

            AssertWrites("expression;", syntax);
        }
    }
}
