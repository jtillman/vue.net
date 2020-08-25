using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsIfStatementSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsProperties()
        {
            var condition = new JsIdentifierNameSyntax(JsSyntaxToken.StringLiteral("condition"));
            var statement = new JsExpressionStatementSyntax(new JsIdentifierNameSyntax(JsSyntaxToken.StringLiteral("trueStatement)")));
            var elseClause = new JsElseClauseSyntax(
                new JsExpressionStatementSyntax(
                    new JsIdentifierNameSyntax(
                        JsSyntaxToken.Identifier("falseStatement"))));

            var syntax = new JsIfStatementSyntax(condition, statement, elseClause);

            Assert.Equal(condition, syntax.Condition);
            Assert.Equal(statement, syntax.Statement);
            Assert.Equal(elseClause, syntax.Else);
        }

        [Fact]
        public void TestWritesSingleConditionStatement()
        {
            var condition = new JsIdentifierNameSyntax(
                JsSyntaxToken.Identifier("condition"));

            var statement = new JsExpressionStatementSyntax(
                new JsIdentifierNameSyntax(
                    JsSyntaxToken.Identifier("trueStatement")));

            var syntax = new JsIfStatementSyntax(condition, statement, null);

            syntax.AssertWrites("if (condition)\r\ntrueStatement;");
        }
    }
}
