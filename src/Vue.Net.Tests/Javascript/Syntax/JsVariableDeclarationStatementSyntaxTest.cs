using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsVariableDeclarationStatementSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsProperties()
        {
            var name = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("x"));
            var initializer = JsExpressionSyntax.Null;

            var syntax = new JsVariableDeclarationStatementSyntax(name, initializer);

            Assert.Equal(name, syntax.Name);
            Assert.Equal(initializer, syntax.Initializer);
        }

        [Fact]
        public void TestWritesDeclarationWithoutInitializer()
        {
            var name = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("x"));

            var syntax = new JsVariableDeclarationStatementSyntax(name, null);

            syntax.AssertWrites("var x;");
        }

        [Fact]
        public void TestWritesDeclarationWithIntializer()
        {
            var name = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("x"));
            var initializer = JsExpressionSyntax.Null;

            var syntax = new JsVariableDeclarationStatementSyntax(name, initializer);

            syntax.AssertWrites("var x = null;");
        }
    }
}
