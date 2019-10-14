using System.IO;
using Vue.Net.Javascript.Syntax;
using Xunit;

namespace Vue.Net.Tests.Javascript.Syntax
{
    public class BaseJsSyntaxTest
    {
        protected void AssertWrites(string expected, JsSyntax syntax)
        {
            using (var stringWriter = new StringWriter())
            {
                syntax.WriteTo(stringWriter);
                Assert.Equal(expected, stringWriter.ToString());
            }
        }
    }
}
