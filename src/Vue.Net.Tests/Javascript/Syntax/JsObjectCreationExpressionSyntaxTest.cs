using System;
using System.Collections.Generic;
using System.Text;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class JsObjectCreationExpressionSyntaxTest
    {
        [Fact]
        public void TestConstructorSetsProperties()
        {
            var properties = new JsObjectPropertySyntax[0];

            var syntax = new JsObjectCreationExpressionSyntax(properties);

            Assert.Equal(properties, syntax.Properties);
        }
    }
}