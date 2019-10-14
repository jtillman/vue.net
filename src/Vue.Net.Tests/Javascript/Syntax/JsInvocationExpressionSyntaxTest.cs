using System;
using System.Collections.Generic;
using System.Text;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsInvocationExpressionSyntaxTest : BaseJsSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsProperties()
        {
            var expression = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("method"));
            var arguments = new JsExpressionSyntax[0];

            var syntax = new JsInvocationExpressionSyntax(expression, arguments);

            Assert.Equal(expression, syntax.Expression);
            Assert.Equal(arguments, syntax.Arguments);
        }

        [Fact]
        public void TestWritesNoArgumentsCorrectly()
        {
            var expression = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("method"));
            var arguments = new JsExpressionSyntax[0];

            var syntax = new JsInvocationExpressionSyntax(expression, arguments);

            AssertWrites("method()", syntax);
        }

        [Fact]
        public void TestWritesSingleArgumentCorrectly()
        {
            var expression = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("method"));
            var arguments = new JsExpressionSyntax[1] {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg1"))
            };

            var syntax = new JsInvocationExpressionSyntax(expression, arguments);

            AssertWrites("method(arg1)", syntax);
        }

        [Fact]
        public void TestWriteMultipleArgumentsCorrectly()
        {
            var expression = new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("method"));
            var arguments = new JsExpressionSyntax[3] {
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg1")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg2")),
                new JsIdentifierNameSyntax(JsSyntaxToken.Identifier("arg3"))
            };

            var syntax = new JsInvocationExpressionSyntax(expression, arguments);

            AssertWrites("method(arg1, arg2, arg3)", syntax);
        }
    }
}