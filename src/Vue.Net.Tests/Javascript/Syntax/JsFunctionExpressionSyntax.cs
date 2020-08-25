using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsFunctionExpressionSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsPropertiesCorrectly()
        {
            var arguments = new JsIdentifierNameSyntax[0];
            var body = new JsBlockSyntax(new JsStatementSyntax[0]);

            var syntax = new JsFunctionExpressionSyntax(arguments, body);

            Assert.Equal(arguments, syntax.ArgumentNames);
            Assert.Equal(body, syntax.Body);
        }

        [Fact]
        public void TestWritesCorrectlyWhenEmpty()
        {
            var arguments = new JsIdentifierNameSyntax[0];
            var body = new JsBlockSyntax(new JsStatementSyntax[0]);

            var syntax = new JsFunctionExpressionSyntax(arguments, body);

            syntax.AssertWrites("function ()\r\n{}");
        }

        [Fact]
        public void TestWritesSingleArgumentCorrectly()
        {
            var arguments = new JsIdentifierNameSyntax[1]
            {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg1"))
            };

            var body = new JsBlockSyntax(new JsStatementSyntax[0]);

            var syntax = new JsFunctionExpressionSyntax(arguments, body);

            syntax.AssertWrites("function (arg1)\r\n{}");
        }

        [Fact]
        public void TestWritesMultipleArgumentsCorrectly()
        {
            var arguments = new JsIdentifierNameSyntax[3]
            {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg1")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg2")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg3"))
            };
            var body = new JsBlockSyntax(new JsStatementSyntax[0]);

            var syntax = new JsFunctionExpressionSyntax(arguments, body);

            syntax.AssertWrites("function (arg1, arg2, arg3)\r\n{}");
        }
    }
}
