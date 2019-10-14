using System;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{

    public class JsIdentifierNameSyntaxTest : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorThrowsOnNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new JsIdentifierNameSyntax(null));
        }

        [Fact]
        public void TestConstructorSetsTokenProperty()
        {
            string name = "idName";
            var token = JsSyntaxToken.Identifier(name);
            var syntax = new JsIdentifierNameSyntax(token);
            Assert.Equal(token, syntax.Token);
        }

        [Fact]
        public void TestWriteToWritesName()
        {
            string name = "idName";
            var token = JsSyntaxToken.Identifier(name);
            var syntax = new JsIdentifierNameSyntax(token);

            AssertWrites(name, syntax);
        }
    }
}
